using ElGuerre.Taskin.Application.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetTodayPomodorosQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetTodayPomodorosQuery, List<TodayPomodoroDto>>
{
    public async Task<List<TodayPomodoroDto>> Handle(GetTodayPomodorosQuery request, CancellationToken cancellationToken)
    {
        var todayStart = DateTime.UtcNow.Date;

        return await context.Pomodoros
            .Where(p => p.StartTime >= todayStart)
            .OrderByDescending(p => p.StartTime)
            .Select(p => new TodayPomodoroDto(
                p.Id,
                p.TaskId,
                p.StartTime,
                p.DurationInMinutes,
                p.CreatedOn.DateTime
            ))
            .ToListAsync(cancellationToken);
    }
}
