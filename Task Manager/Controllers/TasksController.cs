using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Task_Manager.Models;
using Task_Manager.Data;

namespace Task_Manager.Controllers
{
    public class TasksController : Controller
    {
        private readonly ILogger<TasksController> _logger;
        private readonly MVCDataContext _mvcDataContext;

        public TasksController(ILogger<TasksController> logger, MVCDataContext mvcDataContext)
        {
            _logger = logger;
            _mvcDataContext = mvcDataContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetTask(int id)
        {
            try
            {
                var task = await _mvcDataContext.Tasks.FindAsync(id);
                
                if (task == null)
                {
                    return NotFound("Task not found");
                }
                
                return Ok(task);
            }
            catch (Exception ex)
            {
           
                _logger.LogError(ex, "An unexpected error occurred.");
                
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Get all tasks from the database
            var tasks = await _mvcDataContext.Tasks.ToListAsync();
          
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(TaskAddViewModel addTaskRequest)
        {
            var task = new Data.MVCDataContext.Task()
            {
                Id = Guid.NewGuid(),
                Title = addTaskRequest.Title,
                Description = addTaskRequest.Description
            };
            await _mvcDataContext.Tasks.AddAsync(task);
            await _mvcDataContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var task = await _mvcDataContext.Tasks.FirstOrDefaultAsync(x => x.Id == id);

            if (task != null)
            {
                var ViewModel = new UpdateTaskViewModel()
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description
                };
                return View("View", ViewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateTaskViewModel model)
        {
            // Find the task by ID
            var task = await _mvcDataContext.Tasks.FindAsync(model.Id);

            if (task != null)
            {
                task.Title = model.Title;
                task.Description = model.Description;

                await _mvcDataContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // Action to handle deleting a task
        [HttpPost]
        public async Task<IActionResult> Delete(UpdateTaskViewModel model)
        {
            // Find the task by ID
            var task = await _mvcDataContext.Tasks.FindAsync(model.Id);

            if (task != null)
            {
                _mvcDataContext.Tasks.Remove(task);
                await _mvcDataContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
