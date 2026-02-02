using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class ToggleTaskCompletionCommand : IRequest<Domain.Entities.Task>
{
    public Guid Id { get; set; }
}
