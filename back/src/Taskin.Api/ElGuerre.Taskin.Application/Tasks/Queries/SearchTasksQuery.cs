using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;

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

public class TaskFilters
{
    public DomainTaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public Guid? ProjectId { get; set; }
    public string? AssigneeId { get; set; }
    public List<string>? Tags { get; set; }
    public bool? IsOverdue { get; set; }
    public bool? IsCompleted { get; set; }
}

public class TaskListResponse
{
    public List<DomainTask> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}