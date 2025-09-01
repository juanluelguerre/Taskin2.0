using ElGuerre.Taskin.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetTasksByProjectIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTasksByProjectIdQuery, List<Task>>
{
    public async Task<List<Task>> Handle(GetTasksByProjectIdQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await context.Tasks
            .Include(t => t.Pomodoros)
            .Where(t => t.ProjectId == request.ProjectId)
            .ToListAsync(cancellationToken);

        return tasks;
    }
}