using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommand : IRequest<Guid>
{
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public Domain.Entities.TaskStatus Status { get; set; } = Domain.Entities.TaskStatus.Done;
    public DateTime? Deadline { get; set; }
}
