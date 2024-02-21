using CommonUtils.ResultDataResponse;
using TaskTracker_BL.DTOs;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.Interfaces;

public interface IProjectService
{
    Task<ResultData<PagedList<ProjectDto>>> GetProjects(QueryStringParameters projectParameters);
    Task<ResultData<ProjectDto>> GetProjectById(int projectId);

}
