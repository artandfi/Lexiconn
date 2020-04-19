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

namespace Lexiconn.Controllers
{
    public class AccountsController : Controller
    {
        private const string ERR_DUPL_EMAIL = "Користувач із такою адресою вже існує";
        private const string ERR_DUPL_USERNAME = "Користувач з таким ім\'ям вже існує";
        private const string ERR_OLD_PWD = "Неправильний пароль";

        private readonly DBDictionaryContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountsController(UserManager<User> userManager, SignInManager<User> signInManager, DBDictionaryContext context)
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

            ViewBag.Roles = roles.Count > 1 ? roleList.Remove(roleList.Length - 1) : roleList;
            ViewBag.WordCount = _context.Words.Count();
            ViewBag.LangCount = _context.Languages.Count();
            ViewBag.CatCount = _context.Categories.Count();
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
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
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
                    ModelState.AddModelError("Password", "Некоректний логін або пароль");
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
