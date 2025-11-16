using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetPomodorosByTaskIdQuery : IRequest<List<Pomodoro>>
{
    public Guid TaskId { get; set; }
}