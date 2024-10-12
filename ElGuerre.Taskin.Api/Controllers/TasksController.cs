using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Application.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskEntity = ElGuerre.Taskin.Domain.Entities.Task;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public TasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/Tasks/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TaskEntity>> GetTask(Guid id)
    {
        var query = new GetTaskByIdQuery { Id = id };
        var task = await _mediator.Send(query);

        if (task == null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    // GET: api/Tasks?projectId={projectId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasks([FromQuery] Guid projectId)
    {
        var query = new GetTasksByProjectIdQuery { ProjectId = projectId };
        var tasks = await _mediator.Send(query);

        return Ok(tasks);
    }

    // POST: api/Tasks
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskCommand command)
    {
        var taskId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTask), new { id = taskId }, taskId);
    }

    // PUT: api/Tasks/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE: api/Tasks/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(Guid id)
    {
        var command = new DeleteTaskCommand { Id = id };
        await _mediator.Send(command);
        return NoContent();
    }
}