using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class DeleteProjectCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<DeleteProjectCommand>
{
    public async System.Threading.Tasks.Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.Id], cancellationToken);

        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.Id);
        }

        context.Projects.Remove(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics
        metrics.RecordProjectDeleted();
        metrics.UpdateActiveProjects(context.Projects.Count(p => p.Status == ProjectStatus.Active));
    }
}