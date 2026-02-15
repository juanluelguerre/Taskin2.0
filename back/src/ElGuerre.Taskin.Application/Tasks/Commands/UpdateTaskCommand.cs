using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommand : IRequest
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public Domain.Entities.TaskStatus Status { get; set; }
    public Domain.Entities.TaskPriority Priority { get; set; }
    public DateTime? Deadline { get; set; }
    public string? Tags { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public int EstimatedPomodoros { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
