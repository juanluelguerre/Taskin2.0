using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Task = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class CreatePomodoroCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<CreatePomodoroCommand, Guid>
{
    public async Task<Guid> Handle(CreatePomodoroCommand request,
        CancellationToken cancellationToken)
    {
        var task = await context.Tasks.FindAsync([request.TaskId], cancellationToken);
        if (task is null)
        {
            throw new EntityNotFoundException<Task>(request.TaskId);
        }

        Pomodoro pomodoro = new()
        {
            TaskId = request.TaskId,
            StartTime = request.StartTime,
            DurationInMinutes = request.DurationInMinutes,
            Task = task
        };

        context.Pomodoros.Add(pomodoro);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return pomodoro.Id;
    }
}