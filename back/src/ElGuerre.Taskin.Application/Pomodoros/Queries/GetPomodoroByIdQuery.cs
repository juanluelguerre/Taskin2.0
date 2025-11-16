using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetPomodoroByIdQuery : IRequest<Pomodoro>
{
    public Guid Id { get; set; }
}