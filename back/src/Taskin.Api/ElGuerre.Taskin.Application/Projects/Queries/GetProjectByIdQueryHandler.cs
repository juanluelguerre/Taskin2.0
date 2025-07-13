using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectByIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetProjectByIdQuery, Project>
{
    public async Task<Project> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([request.Id], cancellationToken);
        
        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.Id);
        }

        return project;
    }
}