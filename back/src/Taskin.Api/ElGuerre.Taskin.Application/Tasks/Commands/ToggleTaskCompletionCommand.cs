using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class ToggleTaskCompletionCommand : IRequest<DomainTask>
{
    public Guid TaskId { get; set; }
}