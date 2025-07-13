using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;
using DomainTask = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTaskCommand, Guid>
{
    public async Task<Guid> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.ProjectId], cancellationToken);
        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.ProjectId);
        }

        DomainTask task = new()
        {
            Description = request.Description,
            ProjectId = request.ProjectId,
            Status = request.Status,
            Deadline = request.Deadline,
            Project = project
        };

        context.Tasks.Add(task);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}