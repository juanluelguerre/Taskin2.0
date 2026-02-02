using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Application.Tasks.DTOs;
using ElGuerre.Taskin.Application.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController(IMediator mediator) : ControllerBase
{
    // GET: api/Tasks
    [HttpGet]
    public async Task<ActionResult<TaskListResponse>> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int size = 25,
        [FromQuery] Guid? projectId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] string? order = null)
    {
        var query = new GetAllTasksQuery
        {
            Page = page,
            Size = size,
            ProjectId = projectId,
            Status = status,
            Priority = priority,
            Search = search,
            Sort = sort,
            Order = order
        };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    // GET: api/Tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskDetailsDto>> GetTask(Guid id)
    {
        var query = new GetTaskByIdQuery { Id = id };
        var task = await mediator.Send(query);
        return Ok(task);
    }

    // GET: api/Tasks/stats
    [HttpGet("stats")]
    public async Task<ActionResult<TaskStatsDto>> GetTaskStats()
    {
        var query = new GetTaskStatsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    // POST: api/Tasks
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskCommand command)
    {
        var taskId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetTask), new { id = taskId }, taskId);
    }

    // PUT: api/Tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await mediator.Send(command);
        return NoContent();
    }

    // POST: api/Tasks/{id}/toggle-completion
    [HttpPost("{id:guid}/toggle-completion")]
    public async Task<ActionResult<TaskDetailsDto>> ToggleTaskCompletion(Guid id)
    {
        var command = new ToggleTaskCompletionCommand { Id = id };
        var task = await mediator.Send(command);

        // Return a DTO by re-querying
        var query = new GetTaskByIdQuery { Id = id };
        var result = await mediator.Send(query);
        return Ok(result);
    }

    // DELETE: api/Tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var command = new DeleteTaskCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}
