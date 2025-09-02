using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
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

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status != default ? request.Status : task.Status;
        task.Priority = request.Priority != default ? request.Priority : task.Priority;
        task.AssigneeId = request.AssigneeId;
        task.AssigneeName = request.AssigneeName;
        task.DueDate = request.DueDate;
        task.Deadline = request.Deadline ?? task.Deadline;
        task.EstimatedPomodoros = request.EstimatedPomodoros;
        task.Tags = request.Tags ?? task.Tags;
        
        // Handle completion status
        if (request.IsCompleted.HasValue)
        {
            if (request.IsCompleted.Value && !task.IsCompleted)
            {
                task.MarkAsCompleted();
            }
            else if (!request.IsCompleted.Value && task.IsCompleted)
            {
                task.MarkAsIncomplete();
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}