using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetAllTasksQuery : IRequest<TaskListResponse>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 25;
    public Guid? ProjectId { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public string? Order { get; set; }
}
