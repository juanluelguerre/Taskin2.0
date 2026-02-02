using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTasksByProjectIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTasksByProjectIdQuery, List<TaskListDto>>
{
    public async Task<List<TaskListDto>> Handle(GetTasksByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var tasks = await context.Tasks
            .Include(t => t.Project)
            .Where(t => t.ProjectId == request.ProjectId)
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

        return tasks;
    }
}
