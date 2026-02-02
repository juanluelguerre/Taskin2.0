using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTasksByProjectIdQuery : IRequest<List<TaskListDto>>
{
    public Guid ProjectId { get; set; }
}
