using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class ToggleTaskCompletionCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleTaskCompletionCommand, DomainTask>
{
    public async Task<DomainTask> Handle(ToggleTaskCompletionCommand request, CancellationToken cancellationToken)
    {
        var task = await context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException<DomainTask>(request.TaskId);
        }

        // Toggle completion status
        if (task.IsCompleted)
        {
            task.MarkAsIncomplete();
        }
        else
        {
            task.MarkAsCompleted();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return task;
    }
}