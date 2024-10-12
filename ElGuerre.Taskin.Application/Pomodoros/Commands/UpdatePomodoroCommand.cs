using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class UpdatePomodoroCommand : IRequest
{
    public Guid Id { get; set; }
    public DateTime? StartTime { get; set; }
    public int? DurationInMinutes { get; set; }
}