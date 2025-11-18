using ElGuerre.Taskin.Application.Observability;
using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class CreateProjectCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork, TaskinMetrics metrics)
    : IRequestHandler<CreateProjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate,
            ImageUrl = request.ImageUrl,
            BackgroundColor = request.BackgroundColor
        };

        context.Projects.Add(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Record metrics
        metrics.RecordProjectCreated();
        if (project.Status == ProjectStatus.Active)
        {
            metrics.IncrementActiveProjects();
        }

        return project.Id;
    }
}