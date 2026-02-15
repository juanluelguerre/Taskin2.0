using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommand : IRequest<Guid>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public Guid ProjectId { get; set; }
    public Domain.Entities.TaskStatus Status { get; set; } = Domain.Entities.TaskStatus.Todo;
    public Domain.Entities.TaskPriority Priority { get; set; } = Domain.Entities.TaskPriority.Medium;
    public DateTime? Deadline { get; set; }
    public string? Tags { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int EstimatedPomodoros { get; set; }
}
