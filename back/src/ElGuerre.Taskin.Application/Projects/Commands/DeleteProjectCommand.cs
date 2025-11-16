using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class DeleteProjectCommand : IRequest
{
    public Guid Id { get; set; }
}