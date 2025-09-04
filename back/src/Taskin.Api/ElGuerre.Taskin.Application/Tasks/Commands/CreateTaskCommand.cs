using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommand : IRequest<Guid>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public DomainTaskStatus Status { get; set; } = DomainTaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? Deadline { get; set; } // Keep for backward compatibility
    public int? EstimatedPomodoros { get; set; }
    public List<string> Tags { get; set; } = [];
}
