using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTaskByIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTaskByIdQuery, DomainTask>
{
    public async Task<DomainTask> Handle(GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await context.Tasks
            .Include(t => t.Pomodoros)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (task is null)
        {
            throw new EntityNotFoundException<Task>(request.Id);
        }

        return task;
    }
}