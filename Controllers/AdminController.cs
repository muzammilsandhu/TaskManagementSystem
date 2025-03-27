using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Display form to create a task
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(new TaskItem
            {
                Title = string.Empty,
                Description = string.Empty,
                AssignedToUserId = string.Empty,
                AssignedUser = new ApplicationUser()
            });
        }

        // Display form to create a task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(taskItem);
        }

        // Show all tasks in Admin Panel
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();
            return View(tasks);
        }
    }
}
