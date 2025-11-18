using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class UpdateProjectCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<UpdateProjectCommand>
{
    public async System.Threading.Tasks.Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.Id], cancellationToken);

        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.Id);
        }

        var previousStatus = project.Status;
        var wasActive = previousStatus == ProjectStatus.Active;

        project.Name = request.Name;
        project.Description = request.Description;
        if (request.Status.HasValue)
            project.Status = request.Status.Value;
        project.DueDate = request.DueDate;
        project.ImageUrl = request.ImageUrl;
        project.BackgroundColor = request.BackgroundColor;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var isActive = project.Status == ProjectStatus.Active;

        // Update active project count if status changed
        if (!wasActive && isActive)
        {
            metrics.IncrementActiveProjects();
        }
        else if (wasActive && !isActive)
        {
            metrics.DecrementActiveProjects();
        }
    }
}