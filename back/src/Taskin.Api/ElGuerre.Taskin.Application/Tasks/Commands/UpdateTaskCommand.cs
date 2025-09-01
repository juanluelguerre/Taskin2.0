using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommand : IRequest
{
    public Guid Id { get; set; }
    public required string Description { get; set; }
    public Domain.Entities.TaskStatus Status { get; set; }
    public DateTime? Deadline { get; set; }
}