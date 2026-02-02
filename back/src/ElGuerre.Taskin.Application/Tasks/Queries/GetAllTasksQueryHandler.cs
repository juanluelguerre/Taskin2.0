using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;
using TaskPriority = ElGuerre.Taskin.Domain.Entities.TaskPriority;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetAllTasksQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetAllTasksQuery, TaskListResponse>
{
    public async Task<TaskListResponse> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
    {
        var query = context.Tasks
            .Include(t => t.Project)
            .AsQueryable();

        // Apply project filter
        if (request.ProjectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == request.ProjectId.Value);
        }

        // Apply search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                (t.Tags != null && t.Tags.ToLower().Contains(searchTerm)));
        }

        // Apply status filter
        if (!string.IsNullOrEmpty(request.Status))
        {
            if (Enum.TryParse<TaskStatus>(request.Status, true, out var statusEnum))
            {
                query = query.Where(t => t.Status == statusEnum);
            }
        }

        // Apply priority filter
        if (!string.IsNullOrEmpty(request.Priority))
        {
            if (Enum.TryParse<TaskPriority>(request.Priority, true, out var priorityEnum))
            {
                query = query.Where(t => t.Priority == priorityEnum);
            }
        }

        // Apply sorting
        query = request.Sort?.ToLower() switch
        {
            "title" => request.Order?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.Title)
                : query.OrderBy(t => t.Title),
            "status" => request.Order?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.Status)
                : query.OrderBy(t => t.Status),
            "priority" => request.Order?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.Priority)
                : query.OrderBy(t => t.Priority),
            "duedate" => request.Order?.ToLower() == "desc"
                ? query.OrderByDescending(t => t.Deadline)
                : query.OrderBy(t => t.Deadline),
            _ => query.OrderByDescending(t => t.CreatedOn)
        };

        var total = await query.CountAsync(cancellationToken);

        var tasks = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(t => new TaskListDto(
                t.Id,
                t.Title,
                t.Description,
                t.Status.ToString(),
                t.Priority.ToString(),
                t.ProjectId,
                t.Project.Name,
                t.AssigneeId,
                t.AssigneeName,
                t.Deadline,
                t.EstimatedPomodoros,
                t.CompletedPomodoros,
                t.Tags != null ? t.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>(),
                t.IsCompleted,
                t.CompletedAt,
                t.CreatedOn.DateTime,
                t.LastModifiedOn.HasValue ? t.LastModifiedOn.Value.DateTime : t.CreatedOn.DateTime
            ))
            .ToListAsync(cancellationToken);

        return new TaskListResponse(tasks, total, request.Page, request.Size);
    }
}
