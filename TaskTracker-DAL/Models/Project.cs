using Cosmonaut.Attributes;
using System.ComponentModel.DataAnnotations;

namespace TaskTracker_DAL.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public ProjectStatusEnum? Status { get; set; }
    public int Priority { get; set; } = 1;
    public IEnumerable<TaskUnit>? Tasks { get; set; }
}

public enum ProjectStatusEnum
{
    NotStarted = 1,
    Active = 2,
    Completed = 3
}