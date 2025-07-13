using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class UpdateProjectCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProjectCommand>
{
    public async System.Threading.Tasks.Task Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.Id], cancellationToken);
        
        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.Id);
        }

        project.Name = request.Name;
        project.ImageUrl = request.ImageUrl;
        project.BackgroundColor = request.BackgroundColor;

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}