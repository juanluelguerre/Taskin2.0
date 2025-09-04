using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskStatusCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTaskStatusCommand>
{
    public async System.Threading.Tasks.Task Handle(UpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        DomainTask? task = await context.Tasks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (task == null)
        {
            throw new Exception($"Task with ID {request.Id} not found.");
        }

        task.Status = request.Status;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}