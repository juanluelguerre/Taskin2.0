using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class UpdatePomodoroCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<UpdatePomodoroCommand>
{
    public async Task Handle(UpdatePomodoroCommand request, CancellationToken cancellationToken)
    {
        var pomodoro = await context.Pomodoros.FindAsync([request.Id], cancellationToken);
        if (pomodoro is null)
        {
            throw new EntityNotFoundException<Pomodoro>(request.Id);
        }

        var previousDuration = pomodoro.DurationInMinutes;
        pomodoro.StartTime = request.StartTime ?? pomodoro.StartTime;
        pomodoro.DurationInMinutes = request.DurationInMinutes ?? pomodoro.DurationInMinutes;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics if duration was updated (pomodoro completion)
        if (request.DurationInMinutes.HasValue && request.DurationInMinutes.Value != previousDuration)
        {
            metrics.RecordPomodoroDuration(pomodoro.DurationInMinutes);
        }
    }
}