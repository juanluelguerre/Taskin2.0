using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class DuplicateTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<DuplicateTaskCommand, Guid>
{
    public async Task<Guid> Handle(DuplicateTaskCommand request, CancellationToken cancellationToken)
    {
        var originalTask = await context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (originalTask is null)
        {
            throw new EntityNotFoundException<DomainTask>(request.TaskId);
        }

        // Create a duplicate task with all properties except ID and timestamps
        var duplicateTask = new DomainTask
        {
            Title = request.NewTitle ?? $"Copy of {originalTask.Title}",
            Description = originalTask.Description,
            Status = ElGuerre.Taskin.Domain.Entities.TaskStatus.Pending, // Reset status for new task
            Priority = originalTask.Priority,
            ProjectId = originalTask.ProjectId,
            AssigneeId = originalTask.AssigneeId,
            AssigneeName = originalTask.AssigneeName,
            DueDate = originalTask.DueDate,
            Deadline = originalTask.Deadline,
            EstimatedPomodoros = originalTask.EstimatedPomodoros,
            Tags = originalTask.Tags.ToList(), // Create a new list copy
            IsCompleted = false, // Reset completion status
            CompletedAt = null, // Reset completion date
            CompletedPomodoros = 0, // Reset pomodoro count
            Project = originalTask.Project
        };

        context.Tasks.Add(duplicateTask);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return duplicateTask.Id;
    }
}