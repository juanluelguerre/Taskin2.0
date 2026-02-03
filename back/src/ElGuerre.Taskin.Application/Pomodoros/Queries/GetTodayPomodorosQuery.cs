using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public record TodayPomodoroDto(
    Guid Id,
    Guid TaskId,
    DateTime StartTime,
    int DurationInMinutes,
    DateTime CreatedAt
);

public class GetTodayPomodorosQuery : IRequest<List<TodayPomodoroDto>>
{
}
