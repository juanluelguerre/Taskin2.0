using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<UpdateTaskCommand>
{
    public async Task Handle(UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        DomainTask? task =
            await context.Tasks.FindAsync(new object[]
            {
                request.Id
            }, cancellationToken);

        if (task == null)
        {
            throw new Exception("Task not found");
        }

        var previousStatus = task.Status;
        var wasActive = previousStatus == Domain.Entities.TaskStatus.Todo || previousStatus == Domain.Entities.TaskStatus.Doing;

        task.Description = request.Description;
        task.Status = request.Status != default ? request.Status : task.Status;
        task.Deadline = request.Deadline ?? task.Deadline;

        var isActive = task.Status == Domain.Entities.TaskStatus.Todo || task.Status == Domain.Entities.TaskStatus.Doing;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics if task was completed
        if (previousStatus != Domain.Entities.TaskStatus.Done && task.Status == Domain.Entities.TaskStatus.Done)
        {
            metrics.RecordTaskCompleted();
        }

        // Update active tasks counter based on status change
        if (!wasActive && isActive)
        {
            metrics.IncrementActiveTasks();
        }
        else if (wasActive && !isActive)
        {
            metrics.DecrementActiveTasks();
        }
    }
}