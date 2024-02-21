using TaskTracker_BL.Models;
using TaskTracker_DAL.Models;

namespace TaskTracker_BL.DTOs;

public class ProjectDto
{
    public ProjectDto(int projectId, string name, DateOnly? startDate, DateOnly? completionDate, ProjectStatusEnum status, int priority, IEnumerable<TaskUnit>? tasks)
    {
        this.ProjectId = projectId;
        this.Name = name;
        this.StartDate = startDate;
        this.CompletionDate = completionDate;
        this.Status = status;
        this.Priority = priority;

        if (tasks is not null)
        {
            this.Tasks = tasks.Select(x => x.ToTaskDto());
        }
    }

    public int ProjectId { get; set; }
    public string Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public ProjectStatusEnum Status { get; set; }
    public int Priority { get; set; }
    public IEnumerable<TaskDto>? Tasks { get; set; }
}

