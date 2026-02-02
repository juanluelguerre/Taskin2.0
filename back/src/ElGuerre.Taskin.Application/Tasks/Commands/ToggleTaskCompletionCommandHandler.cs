using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class ToggleTaskCompletionCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<ToggleTaskCompletionCommand, DomainTask>
{
    public async Task<DomainTask> Handle(ToggleTaskCompletionCommand request, CancellationToken cancellationToken)
    {
        var task = await context.Tasks.FindAsync([request.Id], cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException<DomainTask>(request.Id);
        }

        if (task.IsCompleted)
        {
            task.IsCompleted = false;
            task.CompletedAt = null;
            task.Status = Domain.Entities.TaskStatus.Todo;
            metrics.IncrementActiveTasks();
        }
        else
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
            task.Status = Domain.Entities.TaskStatus.Done;
            metrics.RecordTaskCompleted();
            metrics.DecrementActiveTasks();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return task;
    }
}
