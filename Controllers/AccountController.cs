using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Lexiconn.Models;
using Lexiconn.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Lexiconn.Supplementary;
using Microsoft.AspNetCore.Authorization;

namespace Lexiconn.Controllers
{
    public class AccountController : Controller
    {
        private const string ERR_DUPL_EMAIL = "Користувач із такою адресою вже існує";
        private const string ERR_DUPL_USERNAME = "Користувач з таким ім\'ям вже існує";
        private const string ERR_LGN_PWD = "Неправильний логін або пароль";
        private const string ERR_OLD_PWD = "Неправильний пароль";
        private const string ERR_UNCONFIRMED = "Ви не підтвердили свій Email";
        private const string SUBJ_CONFIRM = "Підтвердження";
        private const string MSG_END_REG = "Для завершення реєстрації перевірте вашу електронну адресу та перейдіть за посиланням, вказаним у листі.";

        private readonly DBDictionaryContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, DBDictionaryContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            User user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            string roleList = string.Join(", ", roles);

            ViewBag.Roles = roleList;
            ViewBag.WordCount = _context.CategorizedWords.Where(cw => cw.UserName.Equals(User.Identity.Name)).Count();
            ViewBag.LangCount = _context.Languages.Where(cw => cw.UserName.Equals(User.Identity.Name) || cw.UserName == null).Count();
            ViewBag.CatCount = _context.Categories.Where(cw => cw.UserName.Equals(User.Identity.Name) || cw.UserName == null).Count();
            return View(user);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            CheckDuplicates(model);

            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: HttpContext.Request.Scheme);
                    EmailService emailService = new EmailService();

                    await emailService.SendEmailAsync(model.Email, SUBJ_CONFIRM, $"Підтвердіть реєстрацію, перейшовши за <a href='{callbackUrl}'>посиланням</a>.");

                    ViewBag.MessagePopupFlag = 1;
                    ViewBag.Message = MSG_END_REG;
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return RedirectToAction("SignIn", "Account");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            return View(new SignInViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user) && !user.UserName.Equals("admin"))
                    {
                        ModelState.AddModelError("Password", ERR_UNCONFIRMED);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("UserName", ERR_LGN_PWD);
                    return View(model);
                }

                if (!user.UserName.Equals("admin"))
                {
                    await _userManager.AddToRoleAsync(user, "user");
                }
                
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
                
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", ERR_LGN_PWD);
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(User.Identity.Name);
                IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("OldPassword", ERR_OLD_PWD);
            }

            return View(model);
        }

        private void CheckDuplicates(SignUpViewModel model)
        {
            var emailDuplicate = _userManager.Users.Any(u => u.Email.Equals(model.Email));
            var userNameDuplicate = _userManager.Users.Any(u => u.UserName.Equals(model.UserName));

            if (emailDuplicate)
            {
                ModelState.AddModelError("Email", ERR_DUPL_EMAIL);
            }
            if (userNameDuplicate)
            {
                ModelState.AddModelError("UserName", ERR_DUPL_USERNAME);
            }
        }
    }
}
