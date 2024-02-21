using TaskTracker_BL.DTOs;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.Models;

public static class ModelExtensions
{
    public static ProjectDto ToProjectDto(this Project project)
    {
        DateOnly? startDate = null;
        DateOnly? completionDate = null;

        if (project.StartDate != null)
        {
            startDate = DateOnly.FromDateTime((DateTime)project.StartDate!);
        }

        if (project.CompletionDate != null)
        {
            completionDate = DateOnly.FromDateTime((DateTime)project.CompletionDate!);
        }

        return new ProjectDto(
            project.ProjectId,
            project.Name,
            startDate,
            completionDate,
            (ProjectStatusEnum)project.Status!,
            project.Priority,
            project.Tasks!
            );
    }

    public static TaskDto ToTaskDto(this TaskUnit task)
    {
        return new TaskDto(
        task.TaskId,
        task.Name,
        task.Description,
        task.ProjectId
        );
    }
}