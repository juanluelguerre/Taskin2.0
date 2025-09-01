using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Pomodoros.Queries;

public class GetPomodoroByIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetPomodoroByIdQuery, Pomodoro>
{
    public async Task<Pomodoro> Handle(GetPomodoroByIdQuery request,
        CancellationToken cancellationToken)
    {
        var pomodoro = await context.Pomodoros
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (pomodoro is null)
        {
            throw new EntityNotFoundException<Pomodoro>(request.Id);
        }

        return pomodoro;
    }
}