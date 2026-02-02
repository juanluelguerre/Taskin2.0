using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetTodayPomodorosQuery : IRequest<List<Pomodoro>>
{
}
