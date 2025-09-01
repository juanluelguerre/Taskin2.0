using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class DeletePomodoroCommand : IRequest
{
    public Guid Id { get; set; }
}