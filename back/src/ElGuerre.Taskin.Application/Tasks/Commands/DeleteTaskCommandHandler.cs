using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class DeleteTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<DeleteTaskCommand>
{
    public async Task Handle(DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Task? task = await context.Tasks.FindAsync([
            request.Id
        ], cancellationToken);

        if (task == null)
        {
            throw new Exception("Task not found");
        }

        var wasActive = task.Status == Domain.Entities.TaskStatus.Todo || task.Status == Domain.Entities.TaskStatus.Doing;

        context.Tasks.Remove(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics
        metrics.RecordTaskDeleted();
        if (wasActive)
        {
            metrics.DecrementActiveTasks();
        }
    }
}