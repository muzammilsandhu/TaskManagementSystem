using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Data
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Ensure Muzammil is assigned as an Admin
            var adminUser = await userManager.FindByEmailAsync("muzammil@gmail.com");
            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "muzammil",
                    Email = "muzammil@gmail.com",
                };
                var result = await userManager.CreateAsync(admin, "1.Mmuzammil");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
            else
            {
                // Ensure the user has the Admin role
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
