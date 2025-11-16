using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using ElGuerre.Taskin.Domain.SeedWork;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class DeleteProjectCommandHandler(ITaskinDbContext context, IUnitOfWork unitOfWork)
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
    }
}