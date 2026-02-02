using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskByIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTaskByIdQuery, TaskDetailsDto>
{
    public async Task<TaskDetailsDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await context.Tasks
            .Include(t => t.Project)
            .Include(t => t.Pomodoros)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException<DomainTask>(request.Id);
        }

        return new TaskDetailsDto(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.ProjectId,
            task.Project.Name,
            task.AssigneeId,
            task.AssigneeName,
            task.Deadline,
            task.EstimatedPomodoros,
            task.CompletedPomodoros,
            task.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [],
            task.IsCompleted,
            task.CompletedAt,
            task.CreatedOn.DateTime,
            task.LastModifiedOn?.DateTime ?? task.CreatedOn.DateTime,
            task.Pomodoros.Select(p => new PomodoroSummaryDto(
                p.Id,
                p.StartTime,
                p.DurationInMinutes
            )).ToList()
        );
    }
}
