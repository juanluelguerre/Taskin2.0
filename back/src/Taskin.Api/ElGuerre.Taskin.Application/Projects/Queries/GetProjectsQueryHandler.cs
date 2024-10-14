using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetProjectsQuery, List<Project>>
{
    public async Task<List<Project>> Handle(GetProjectsQuery request,
        CancellationToken cancellationToken)
    {
        return await context.Projects
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
    }
}