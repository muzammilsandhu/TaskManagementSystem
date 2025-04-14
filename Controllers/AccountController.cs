using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Models;
using TaskManagementSystem.ViewModels;

namespace TaskManagementSystem.Controllers
    {
    public class AccountController : Controller
        {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController ( UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager , RoleManager<IdentityRole> roleManager )
            {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            }

        // 🔹 Register User
        [HttpGet]
        public IActionResult Register () => View ();

        [HttpPost]
        public async Task<IActionResult> Register ( RegisterViewModel model )
            {
            if (ModelState.IsValid)
                {
                var user = new ApplicationUser { UserName = model.Email , Email = model.Email };

                var result = await _userManager.CreateAsync (user , model.Password);
                if (result.Succeeded)
                    {
                    // Assign default role if it doesn't exist
                    if (!await _roleManager.RoleExistsAsync ("User"))
                        {
                        await _roleManager.CreateAsync (new IdentityRole ("User"));
                        }
                    await _userManager.AddToRoleAsync (user , "User");

                    // Assign default permissions based on role
                    var claims = new List<Claim>
            {
                new Claim("Permission", AppPermissions.CanViewAllTasks),
                 new Claim("Permission", AppPermissions.CanCreateTask),
                 new Claim("Permission", AppPermissions.CanEditTask),
                 new Claim("Permission", AppPermissions.CanDeleteTask)
            };

                    // Adding claims to the user
                    foreach (var claim in claims)
                        {
                        await _userManager.AddClaimAsync (user , claim);
                        }

                    // Sign the user in
                    await _signInManager.SignInAsync (user , isPersistent: false);
                    return RedirectToAction ("Index" , "Home");
                    }

                // Add errors to the model state
                foreach (var error in result.Errors)
                    {
                    ModelState.AddModelError ("" , error.Description);
                    }
                }
            return View (model);
            }



        // 🔹 Login User
        [HttpGet]
        public IActionResult Login () => View ();

        [HttpPost]
        public async Task<IActionResult> Login ( LoginViewModel model )
            {
            if (ModelState.IsValid)
                {
                var result = await _signInManager.PasswordSignInAsync (model.Email , model.Password , model.RememberMe , false);
                if (result.Succeeded)
                    {
                    return RedirectToAction ("Index" , "Home");
                    }
                ModelState.AddModelError ("" , "Invalid login attempt.");
                }
            return View (model);
            }

        // 🔹 Logout User
        [HttpPost]
        public async Task<IActionResult> Logout ()
            {
            await _signInManager.SignOutAsync ();
            return RedirectToAction ("Index" , "Home");
            }
        }
    }
