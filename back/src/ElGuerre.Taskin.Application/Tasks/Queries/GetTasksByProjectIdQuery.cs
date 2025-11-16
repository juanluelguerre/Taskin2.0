using MediatR;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTasksByProjectIdQuery : IRequest<List<Task>>
{
    public Guid ProjectId { get; set; }
}