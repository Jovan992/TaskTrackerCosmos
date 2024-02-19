using CommonUtils.ResultDataResponse;
using Microsoft.AspNetCore.Mvc;
using TaskTracker_BL.DTOs;
using TaskTracker_BL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskService taskService) : ControllerBase
    {
        private readonly ITaskService taskService = taskService;
        private readonly string idNotGreaterThanZero = "Id must be greater than 0.";

        // GET: api/Tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskParameters taskParameters)
        {
            ResultData<PagedList<TaskDto>> tasks = await taskService.GetTasks(taskParameters);

            if (tasks is NotFoundResultData<PagedList<TaskDto>>)
            {
                return tasks.ToNotFoundActionResult();
            }

            return tasks.ToOkActionResult();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            if (id < 1)
            {
                return BadRequest(idNotGreaterThanZero);
            }

            ResultData<TaskDto> task = await taskService.GetTaskById(id);

            if (task is NotFoundResultData<TaskDto>)
            {
                return task.ToNotFoundActionResult();
            }

            return task.ToOkActionResult();
        }
    }
}