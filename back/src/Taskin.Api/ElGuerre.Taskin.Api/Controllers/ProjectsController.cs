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
    public async Task<IActionResult> GetProjects([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = new GetProjectsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var projects = await mediator.Send(query);
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectCommand command)
    {
        var projectId = await mediator.Send(command);
        return CreatedAtAction(nameof(this.GetProjects), new { id = projectId }, null);
    }
}
