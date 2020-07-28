using Lexiconn.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LibraryMVC
{
    public class RoleInitializer
    {
        private const string _adminEmail = "misdispel@gmail.com";
        private const string _password = "password";

        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (await userManager.FindByEmailAsync(_adminEmail) == null)
            {
                User admin = new User { Email = _adminEmail, UserName = "admin" };
                IdentityResult result = await userManager.CreateAsync(admin, _password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
