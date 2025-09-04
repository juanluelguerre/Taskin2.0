using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommand : IRequest
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DomainTaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? Deadline { get; set; } // Keep for backward compatibility
    public int? EstimatedPomodoros { get; set; }
    public List<string> Tags { get; set; } = [];
    public bool? IsCompleted { get; set; }
}