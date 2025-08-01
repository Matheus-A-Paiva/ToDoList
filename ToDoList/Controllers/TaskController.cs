using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Dtos;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string status = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var query = _context.Tasks.Where(t => t.UserId == user.Id);
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status == status);
            }
            var userTasks = await query.ToListAsync();

            return View(userTasks);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = await _userManager.GetUserAsync(User);

            var userTask = new UserTask
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                DueDate = dto.DueDate,
                UserId = user.Id,
            };

            _context.Tasks.Add(userTask);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var userTask = await GetUserTaskAsync(id);
            if (userTask == null)
            {
                return NotFound();
            }

            var dto = new UpdateTaskDto
            {
                Id = userTask.Id,
                Title = userTask.Title,
                Description = userTask.Description,
                Status = userTask.Status,
                DueDate = userTask.DueDate
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var userTask = await GetUserTaskAsync(dto.Id);
            if (userTask == null)
            {
                return NotFound();
            }

            userTask.Title = dto.Title;
            userTask.Description = dto.Description;
            userTask.Status = dto.Status;
            userTask.DueDate = dto.DueDate;

            _context.Tasks.Update(userTask);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            UserTask userTask = await GetUserTaskAsync(id);
            if (userTask == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(userTask);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<UserTask> GetUserTaskAsync(int taskId)
        {
            var userId = _userManager.GetUserId(User);
            return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        }
    }
}
