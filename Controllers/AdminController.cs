using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
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
        [PermissionAuthorize (AppPermissions.CanCreateTask)]
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

        // GET: Edit Task
        [PermissionAuthorize (AppPermissions.CanEditTask)]
        public async Task<IActionResult> Edit(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound();

            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(task);
        }

        // POST: Edit Task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem taskItem)
        {
            if (id != taskItem.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _userManager.Users.ToListAsync();
            return View(taskItem);
        }

        // GET: Delete Task
        [PermissionAuthorize (AppPermissions.CanDeleteTask)]
        public async Task<IActionResult> Delete(int id)
        {
            var task = await _context.Tasks.Include(t => t.AssignedUser).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
                return NotFound();

            return View(task);
        }

        // POST: Confirm Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Show all tasks in Admin Panel
        [PermissionAuthorize (AppPermissions.CanViewAllTasks)]
        public async Task<IActionResult> Index()
        {
            var tasks = await _context.Tasks.Include(t => t.AssignedUser).ToListAsync();
            return View(tasks);
        }
    }
}
