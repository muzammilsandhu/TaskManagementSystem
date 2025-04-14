using Microsoft.AspNetCore.Identity;
using TaskManagementSystem.Models;
using System.Security.Claims;

namespace TaskManagementSystem.Data
    {
    public class RoleSeeder
        {
        public static async Task SeedRolesAndAdminAsync ( IServiceProvider serviceProvider )
            {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>> ();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>> ();

            string[] roleNames = { "Admin" , "User" };
            foreach (var roleName in roleNames)
                {
                if (!await roleManager.RoleExistsAsync (roleName))
                    {
                    await roleManager.CreateAsync (new IdentityRole (roleName));
                    }
                }

            var adminUser = await userManager.FindByEmailAsync ("muzammil@gmail.com");
            if (adminUser == null)
                {
                var admin = new ApplicationUser
                    {
                    UserName = "muzammil" ,
                    Email = "muzammil@gmail.com" ,
                    };
                var result = await userManager.CreateAsync (admin , "1.Mmuzammil");
                if (result.Succeeded)
                    {
                    await userManager.AddToRoleAsync (admin , "Admin");
                    adminUser = admin;
                    }
                }
            else
                {
                if (!await userManager.IsInRoleAsync (adminUser , "Admin"))
                    {
                    await userManager.AddToRoleAsync (adminUser , "Admin");
                    }
                }

            // ✅ Assign permission claims to the admin user
            var permissions = new List<string>
            {
                AppPermissions.CanCreateTask,
                AppPermissions.CanEditTask,
                AppPermissions.CanDeleteTask,
                AppPermissions.CanViewAllTasks
            };

            // Ensure adminUser is not null before proceeding
            if (adminUser != null)
                {
                var existingClaims = await userManager.GetClaimsAsync (adminUser);
                foreach (var permission in permissions)
                    {
                    if (!existingClaims.Any (c => c.Type == "Permission" && c.Value == permission))
                        {
                        await userManager.AddClaimAsync (adminUser , new Claim ("Permission" , permission));
                        }
                    }
                }
            }
        }
    }
