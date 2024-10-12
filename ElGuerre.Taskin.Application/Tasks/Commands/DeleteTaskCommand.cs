using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class DeleteTaskCommand : IRequest
{
    public Guid Id { get; set; }
}