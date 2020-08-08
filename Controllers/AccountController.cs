using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Lexiconn.Models;
using Lexiconn.ViewModels;
using Lexiconn.Supplementary;

namespace Lexiconn.Controllers
{
    public class AccountController : Controller
    {
        private const string ERR_DUPL_EMAIL = "Користувач із такою адресою вже існує";
        private const string ERR_DUPL_USERNAME = "Користувач з таким ім\'ям вже існує";
        private const string ERR_LGN_PWD = "Неправильний логін або пароль";
        private const string ERR_PWD = "Неправильний пароль";
        private const string ERR_UNCONFIRMED = "Ви не підтвердили свій Email";
        private const string MSG_END_REG = "Для завершення реєстрації перевірте вашу електронну адресу та перейдіть за посиланням, вказаним у листі.";
        private const string HDR_CONFIRM = "Підтвердження";

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
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            InitProfileViewBag(roles);
            return View(user);
        }

        private void InitProfileViewBag(ICollection<string> roles)
        {
            ViewBag.Roles = string.Join(", ", roles);
            ViewBag.WordCount = _context.CategorizedWords.Where(cw => cw.UserName.Equals(User.Identity.Name)).Count();
            ViewBag.LangCount = _context.Languages.Where(cw => cw.UserName.Equals(User.Identity.Name)).Count();
            ViewBag.CatCount = _context.Categories.Where(cw => cw.UserName.Equals(User.Identity.Name)).Count();
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
                var user = new User { Email = model.Email, UserName = model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    SendConfirmationEmail(user, model);
                    InitRegistrationPopup();
                    return RedirectToAction("Index", "Home");
                }
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

        private async void SendConfirmationEmail(User user, SignUpViewModel model)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, HttpContext.Request.Scheme);
            var emailService = new EmailService();
            await emailService.SendEmailAsync(model.Email, HDR_CONFIRM, $"Підтвердіть реєстрацію, перейшовши за <a href='{ callbackUrl }'>посиланням</a>.");
        }

        private void InitRegistrationPopup()
        {
            ViewBag.MessagePopupFlag = 1;
            ViewBag.Message = MSG_END_REG;
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

            return View("Error");
        }

        public IActionResult SignIn() => View(new SignInViewModel());

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                
                if (!await ValidateUser(user))
                {
                    return View(model);
                }

                if (await SignIn(user, model))
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("Password", ERR_LGN_PWD);
            }
            return View(model);
        }

        private async Task<bool> ValidateUser(User user)
        {
            if (user == null)
            {
                ModelState.AddModelError("UserName", ERR_LGN_PWD);
                return false;
            }

            if (!await _userManager.IsEmailConfirmedAsync(user) && !user.UserName.Equals("admin"))
            {
                ModelState.AddModelError("Password", ERR_UNCONFIRMED);
                return false;
            }

            return true;
        }

        private async Task<bool> SignIn(User user, SignInViewModel model)
        {
            if (!user.UserName.Equals("admin"))
            {
                await _userManager.AddToRoleAsync(user, "user");
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            return result.Succeeded;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("OldPassword", ERR_PWD);
            }

            return View(model);
        }
    }
}
