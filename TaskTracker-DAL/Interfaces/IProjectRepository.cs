using CommonUtils.ResultDataResponse;
using TaskTracker_DAL.Models;

namespace TaskTracker_DAL.Interfaces
{
    public interface IProjectRepository
    {
        Task<ResultData<PagedList<Project>>> GetProjects(QueryStringParameters projectParameters);
        Task<ResultData<Project>> GetProjectById(int projectId);
    }
}
