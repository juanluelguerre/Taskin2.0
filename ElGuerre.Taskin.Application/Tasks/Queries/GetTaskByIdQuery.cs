using MediatR;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskByIdQuery : IRequest<Task>
{
    public Guid Id { get; set; }
}