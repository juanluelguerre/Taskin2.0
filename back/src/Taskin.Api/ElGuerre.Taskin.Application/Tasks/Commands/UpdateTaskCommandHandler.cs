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

        task.Description = request.Description;
        task.Status = request.Status != default ? request.Status : task.Status;
        task.Deadline = request.Deadline ?? task.Deadline;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}