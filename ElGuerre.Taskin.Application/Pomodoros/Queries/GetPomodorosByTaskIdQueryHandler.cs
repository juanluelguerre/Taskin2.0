using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetPomodorosByTaskIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetPomodorosByTaskIdQuery, List<Pomodoro>>
{
    public async Task<List<Pomodoro>> Handle(GetPomodorosByTaskIdQuery request,
        CancellationToken cancellationToken)
    {
        var pomodoros = await context.Pomodoros
            .Where(p => p.TaskId == request.TaskId)
            .ToListAsync(cancellationToken);

        return pomodoros;
    }
}