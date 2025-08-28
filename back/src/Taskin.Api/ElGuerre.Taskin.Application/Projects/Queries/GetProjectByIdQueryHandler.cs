using ElGuerre.Taskin.Application.Data;
using ElGuerre.Taskin.Application.Exceptions;
using ElGuerre.Taskin.Application.Projects.DTOs;
using ElGuerre.Taskin.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectByIdQueryHandler(ITaskinDbContext context)
    : IRequestHandler<GetProjectByIdQuery, ProjectDetailsDto>
{
    public async Task<ProjectDetailsDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        
        if (project is null)
        {
            throw new EntityNotFoundException<Project>(request.Id);
        }

        var taskSummaries = project.Tasks.Select(t => new TaskSummaryDto(
            t.Id,
            t.Description ?? "",
            t.Status.ToString().ToLower(),
            "medium" // Priority not implemented yet
        )).ToList();

        var completedTasks = project.Tasks.Count(t => t.Status == Domain.Entities.TaskStatus.Done);
        var progress = project.Tasks.Count > 0 ? (int)Math.Round((double)completedTasks / project.Tasks.Count * 100) : 0;

        return new ProjectDetailsDto(
            project.Id,
            project.Name,
            project.Description,
            project.Status.ToString().ToLower(),
            progress,
            project.Tasks.Count,
            completedTasks,
            project.DueDate ?? DateTime.Now.AddDays(30),
            project.ImageUrl,
            project.BackgroundColor,
            project.CreatedOn.DateTime,
            project.LastModifiedOn?.DateTime ?? project.CreatedOn.DateTime,
            taskSummaries,
            new List<TeamMemberDto>() // Team members not implemented yet
        );
    }
}