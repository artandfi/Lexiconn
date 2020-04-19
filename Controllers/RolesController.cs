using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconn.Models;
using Lexiconn.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private const string ERR_DUPL_ROLE = "Така роль вже існує";
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_roleManager.Roles.Any(r => r.Name.Equals(model.Name)))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Name));
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("Name", ERR_DUPL_ROLE);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
    }
}
