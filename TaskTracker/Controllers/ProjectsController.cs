using CommonUtils.ResultDataResponse;
using Microsoft.AspNetCore.Mvc;
using TaskTracker_BL.DTOs;
using TaskTracker_BL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService projectService) : ControllerBase
    {
        private readonly string idNotGreaterThanZero = "Id must be greater than 0.";

        private readonly IProjectService projectService = projectService;

        // GET: api/Projects
        [HttpGet]
        // PagedList<ProjectDto>
        public async Task<IActionResult> GetProjects([FromQuery] QueryStringParameters projectParameters)
        {
            ResultData<PagedList<ProjectDto>> projects = await projectService.GetProjects(projectParameters);

            if (projects is NotFoundResultData<PagedList<ProjectDto>>)
            {
                return projects.ToNotFoundActionResult();
            }

            return projects.ToOkActionResult();
        }

        // GET: api/Projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            if (id < 1)
            {
                return BadRequest(idNotGreaterThanZero);
            }

            ResultData<ProjectDto> project = await projectService.GetProjectById(id);

            if (project is NotFoundResultData<ProjectDto>)
            {
                return project.ToNotFoundActionResult();
            }

            return project.ToOkActionResult();
        }

    }
}