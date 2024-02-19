using CommonUtils.ResultDataResponse;
using TaskTracker_BL.DTOs;
using TaskTracker_BL.Interfaces;
using TaskTracker_BL.Models;
using TaskTracker_DAL.Interfaces;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.Services
{
    public class TaskService(ITaskRepository taskRepository) : ITaskService
    {
        private readonly ITaskRepository taskRepository = taskRepository;

        public async Task<ResultData<PagedList<TaskDto>>> GetTasks(TaskParameters taskParameters)
        {
            ResultData<PagedList<TaskUnit>> result = await taskRepository.GetTasks(taskParameters);

            if (result is NotFoundResultData<PagedList<TaskUnit>>)
            {
                return new NotFoundResultData<PagedList<TaskDto>>(result.Message);
            }

            PagedList<TaskDto> taskDtoList = PagedList<TaskDto>.ToPagedList(
                result.Data!.Items
                .Select(x => x.ToTaskDto()).AsQueryable(),
                result.Data.CurrentPage,
                result.Data.PageSize);

            return new OkResultData<PagedList<TaskDto>>(taskDtoList);
        }

        public async Task<ResultData<TaskDto>> GetTaskById(int taskId)
        {
            ResultData<TaskUnit> taskFound = await taskRepository.GetTaskById(taskId);

            if (taskFound is NotFoundResultData<TaskUnit>)
            {
                return new NotFoundResultData<TaskDto>(taskFound.Message);
            }

            return new OkResultData<TaskDto>(taskFound.Data!.ToTaskDto());
        }
    }
}