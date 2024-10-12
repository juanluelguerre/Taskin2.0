using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using Task = System.Threading.Tasks.Task;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class DeletePomodoroCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePomodoroCommand>
{
    public async Task Handle(DeletePomodoroCommand request, CancellationToken cancellationToken)
    {
        var pomodoro = await context.Pomodoros.FindAsync([request.Id], cancellationToken);
        if (pomodoro is null)
        {
            throw new EntityNotFoundException<Pomodoro>(request.Id);
        }

        context.Pomodoros.Remove(pomodoro);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}