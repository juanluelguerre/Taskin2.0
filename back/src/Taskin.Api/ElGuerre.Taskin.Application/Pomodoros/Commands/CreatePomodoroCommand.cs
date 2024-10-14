using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class CreatePomodoroCommand : IRequest<Guid>
{
    public Guid TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }
}