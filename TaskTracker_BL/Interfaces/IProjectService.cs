using CommonUtils.ResultDataResponse;
using TaskTracker_BL.DTOs;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.Interfaces;

public interface IProjectService
{
    Task<ResultData<PagedList<ProjectDto>>> GetProjects(QueryStringParameters queryParameters);
    Task<ResultData<ProjectDto>> GetProjectById(int projectId);

}
