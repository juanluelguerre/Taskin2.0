using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.DTOs;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetProjectsQuery, CollectionResponse<ProjectListDto>>
{
    public async Task<CollectionResponse<ProjectListDto>> Handle(GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Projects.Include(p => p.Tasks).AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm) || 
                                   (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
        }

        // Apply status filter
        if (!string.IsNullOrEmpty(request.Status) && request.Status != "all")
        {
            if (Enum.TryParse<ProjectStatus>(request.Status, true, out var statusEnum))
            {
                query = query.Where(p => p.Status == statusEnum);
            }
        }

        // Apply sorting
        query = request.Sort?.ToLower() switch
        {
            "name" => request.Order?.ToLower() == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "status" => request.Order?.ToLower() == "desc" ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
            "duedate" => request.Order?.ToLower() == "desc" ? query.OrderByDescending(p => p.DueDate) : query.OrderBy(p => p.DueDate),
            "created" => request.Order?.ToLower() == "desc" ? query.OrderByDescending(p => p.CreatedOn) : query.OrderBy(p => p.CreatedOn),
            _ => query.OrderByDescending(p => p.CreatedOn) // Default sort
        };

        var total = await query.CountAsync(cancellationToken);

        var projects = await query
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(p => new ProjectListDto(
                p.Id,
                p.Name,
                p.Description,
                p.Status.ToString().ToLower(),
                CalculateProgress(p.Tasks),
                p.Tasks.Count,
                p.Tasks.Count(t => t.Status == Domain.Entities.TaskStatus.Done),
                p.DueDate ?? DateTime.Now.AddDays(30),
                p.ImageUrl,
                p.BackgroundColor,
                p.CreatedOn.DateTime,
                p.LastModifiedOn.HasValue ? p.LastModifiedOn.Value.DateTime : p.CreatedOn.DateTime
            ))
            .ToListAsync(cancellationToken);

        return new CollectionResponse<ProjectListDto>(projects, total, request.Page, request.Size);
    }

    private static int CalculateProgress(ICollection<Domain.Entities.Task> tasks)
    {
        if (!tasks.Any()) return 0;
        
        var completedTasks = tasks.Count(t => t.Status == Domain.Entities.TaskStatus.Done);
        return (int)Math.Round((double)completedTasks / tasks.Count * 100);
    }
}