using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetTodayPomodorosQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTodayPomodorosQuery, List<Pomodoro>>
{
    public async Task<List<Pomodoro>> Handle(GetTodayPomodorosQuery request, CancellationToken cancellationToken)
    {
        var todayStart = DateTime.UtcNow.Date;

        return await context.Pomodoros
            .Include(p => p.Task)
            .Where(p => p.StartTime >= todayStart)
            .OrderByDescending(p => p.StartTime)
            .ToListAsync(cancellationToken);
    }
}
