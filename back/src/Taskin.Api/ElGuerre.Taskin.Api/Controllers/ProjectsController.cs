using ElGuerre.Taskin.Application.Projects.Commands;
using ElGuerre.Taskin.Application.Projects.DTOs;
using ElGuerre.Taskin.Application.Projects.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CollectionResponse<ProjectListDto>>> GetProjects(
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? sort = null,
        [FromQuery] string? order = null)
    {
        var query = new GetProjectsQuery
        {
            Page = page,
            Size = size,
            Search = search,
            Status = status,
            Sort = sort,
            Order = order
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDetailsDto>> GetProject(Guid id)
    {
        var query = new GetProjectByIdQuery { Id = id };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ActionResponse>> CreateProject(
        [FromBody] CreateProjectCommand command)
    {
        Guid result = await mediator.Send(command);
        ActionResponse response = new ActionResponse(result, "Project created successfully");
        return CreatedAtAction(nameof(GetProject), new { id = result }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ActionResponse>> UpdateProject(
        Guid id, [FromBody] UpdateProjectCommand command)
    {
        command.Id = id;
        await mediator.Send(command);
        var response = new ActionResponse(id, "Project updated successfully");
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ActionResponse>> DeleteProject(Guid id)
    {
        var command = new DeleteProjectCommand { Id = id };
        await mediator.Send(command);
        var response = new ActionResponse(id, "Project deleted successfully");
        return Ok(response);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ProjectStatsDto>> GetProjectStats()
    {
        var query = new GetProjectStatsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }
}