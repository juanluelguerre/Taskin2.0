using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class SearchTasksQuery : IRequest<TaskListResponse>
{
    public string? Query { get; set; }
    public TaskFilters? Filters { get; set; }
    public string SortBy { get; set; } = "CreatedAt";
    public string SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 25;
}