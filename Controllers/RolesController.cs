using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lexiconn.Models;
using Lexiconn.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index() => View(_roleManager.Roles.ToList());
    }
}
