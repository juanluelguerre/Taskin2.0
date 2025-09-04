using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainTaskStatus = ElGuerre.Taskin.Domain.Entities.TaskStatus;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class BulkUpdateTaskStatusCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<BulkUpdateTaskStatusCommand>
{
    public async System.Threading.Tasks.Task Handle(BulkUpdateTaskStatusCommand request, CancellationToken cancellationToken)
    {
        var tasks = await context.Tasks
            .Where(t => request.TaskIds.Contains(t.Id))
            .ToListAsync(cancellationToken);

        foreach (var task in tasks)
        {
            task.Status = request.Status;
            
            // Update completion status based on the new status
            if (request.Status == DomainTaskStatus.Completed || request.Status == DomainTaskStatus.Done)
            {
                if (!task.IsCompleted)
                {
                    task.MarkAsCompleted();
                }
            }
            else if (task.IsCompleted)
            {
                task.MarkAsIncomplete();
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}