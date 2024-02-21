namespace TaskTracker_BL.DTOs;

public class TaskDto
{
    public TaskDto(int taskId, string name, string description, int projectId)
    {
        this.TaskId = taskId;
        this.Name = name;
        this.Description = description;
        this.ProjectId = projectId;
    }
    public int TaskId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int ProjectId { get; set; }
}

