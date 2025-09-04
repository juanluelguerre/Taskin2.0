using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class SearchTasksQueryHandler(ITaskinDbContext context)
    : IRequestHandler<SearchTasksQuery, TaskListResponse>
{
    public async System.Threading.Tasks.Task<TaskListResponse> Handle(SearchTasksQuery request, CancellationToken cancellationToken)
    {
        var query = context.Tasks
            .Include(t => t.Project)
            .AsQueryable();

        // Apply text search
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var searchTerm = request.Query.ToLower();
            query = query.Where(t => 
                t.Title.ToLower().Contains(searchTerm) ||
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                t.Tags.Any(tag => tag.ToLower().Contains(searchTerm)));
        }

        // Apply filters
        if (request.Filters != null)
        {
            if (request.Filters.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Filters.Status.Value);
            }

            if (request.Filters.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == request.Filters.Priority.Value);
            }

            if (request.Filters.ProjectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == request.Filters.ProjectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Filters.AssigneeId))
            {
                query = query.Where(t => t.AssigneeId == request.Filters.AssigneeId);
            }

            if (request.Filters.IsCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == request.Filters.IsCompleted.Value);
            }

            if (request.Filters.IsOverdue == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(t => 
                    t.DueDate.HasValue && 
                    t.DueDate.Value < now && 
                    !t.IsCompleted);
            }

            if (request.Filters.Tags != null && request.Filters.Tags.Count > 0)
            {
                query = query.Where(t => 
                    request.Filters.Tags.Any(tag => t.Tags.Contains(tag)));
            }
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortDirection);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .ToListAsync(cancellationToken);

        return new TaskListResponse
        {
            Items = items,
            TotalCount = totalCount,
            CurrentPage = request.Page,
            PageSize = request.Size
        };
    }

    private static IQueryable<DomainTask> ApplySorting(IQueryable<DomainTask> query, string sortBy, string sortDirection)
    {
        var isAscending = sortDirection.ToLower() == "asc";

        return sortBy.ToLower() switch
        {
            "title" => isAscending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
            "status" => isAscending ? query.OrderBy(t => t.Status) : query.OrderByDescending(t => t.Status),
            "priority" => isAscending ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),
            "duedate" => isAscending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate),
            "createdat" => isAscending ? query.OrderBy(t => t.CreatedOn) : query.OrderByDescending(t => t.CreatedOn),
            "updatedat" => isAscending ? query.OrderBy(t => t.LastModifiedOn) : query.OrderByDescending(t => t.LastModifiedOn),
            _ => query.OrderByDescending(t => t.CreatedOn) // Default sorting
        };
    }
}