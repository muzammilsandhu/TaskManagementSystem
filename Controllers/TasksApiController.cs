using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Controllers
{
    [Produces("application/json")]
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class TasksApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetTasks()
        {
            var tasks = _context.Tasks
                .Select(task => new TaskItemDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    AssignedToUserId = task.AssignedToUserId,
                    AssignedToUserName = task.AssignedUser != null ? task.AssignedUser.UserName : null
                }).ToList();

            return Ok(tasks);
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskItemDTO dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                AssignedToUserId = dto.AssignedToUserId
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            dto.Id = task.Id; // return back the created Id

            return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItemDTO dto)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.AssignedToUserId = dto.AssignedToUserId;

            _context.SaveChanges();

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task == null) return NotFound();

            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return Ok("Task deleted");
        }
    }
}