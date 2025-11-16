using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Projects.DTOs;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectStatsQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetProjectStatsQuery, ProjectStatsDto>
{
    public async Task<ProjectStatsDto> Handle(GetProjectStatsQuery request, CancellationToken cancellationToken)
    {
        var projects = await context.Projects.ToListAsync(cancellationToken);
        
        var total = projects.Count;
        var active = projects.Count(p => p.Status == ProjectStatus.Active);
        var completed = projects.Count(p => p.Status == ProjectStatus.Completed);
        var onHold = projects.Count(p => p.Status == ProjectStatus.OnHold);
        
        return new ProjectStatsDto(total, active, completed, onHold);
    }
}