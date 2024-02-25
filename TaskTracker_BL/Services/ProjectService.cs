using CommonUtils.ResultDataResponse;
using TaskTracker_BL.DTOs;
using TaskTracker_BL.Interfaces;
using TaskTracker_BL.Models;
using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.Services;

public class ProjectService(IProjectRepository projectRepository) : IProjectService
{
    private readonly IProjectRepository projectRepository = projectRepository;

    public async Task<ResultData<PagedList<ProjectDto>>> GetProjects(QueryStringParameters queryParameters)
    {
        ResultData<PagedList<Project>> result = await projectRepository.GetProjects(queryParameters);

        if (result is NotFoundResultData<PagedList<Project>>)
        {
            return new NotFoundResultData<PagedList<ProjectDto>>(result.Message);
        }

        PagedList<ProjectDto> projectDtosList = PagedList<ProjectDto>.ToPagedList(
            result.Data!.Items
            .Select(x => x.ToProjectDto())
            .AsQueryable(),
            result.Data.CurrentPage,
            result.Data.PageSize);

        return new OkResultData<PagedList<ProjectDto>>(projectDtosList);
    }

    public async Task<ResultData<ProjectDto>> GetProjectById(int projectId)
    {
        ResultData<Project> projectFound = await projectRepository.GetProjectById(projectId);

        if (projectFound is NotFoundResultData<Project>)
        {
            return new NotFoundResultData<ProjectDto>(projectFound.Message);
        }

        return new OkResultData<ProjectDto>(projectFound.Data!.ToProjectDto());
    }
}