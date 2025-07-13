using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjects(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProjectsQuery { PageNumber = pageNumber, PageSize = pageSize };
        var projects = await mediator.Send(query);
        return Ok(projects);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var query = new GetProjectByIdQuery { Id = id };
        var project = await mediator.Send(query);
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        var projectId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetProject), new { id = projectId }, null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var command = new DeleteProjectCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}