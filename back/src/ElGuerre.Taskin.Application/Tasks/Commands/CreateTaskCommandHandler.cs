using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<CreateTaskCommand, Guid>
{
    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.ProjectId], cancellationToken);
        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.ProjectId);
        }

        DomainTask task = new()
        {
            Title = request.Title,
            Description = request.Description,
            ProjectId = request.ProjectId,
            Status = request.Status,
            Priority = request.Priority,
            Deadline = request.Deadline,
            Tags = request.Tags,
            AssigneeId = request.AssigneeId,
            AssigneeName = request.AssigneeName,
            EstimatedPomodoros = request.EstimatedPomodoros,
            Project = project
        };

        context.Tasks.Add(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics
        metrics.RecordTaskCreated();
        if (task.Status == Domain.Entities.TaskStatus.Todo || task.Status == Domain.Entities.TaskStatus.Doing)
        {
            metrics.IncrementActiveTasks();
        }

        return task.Id;
    }
}