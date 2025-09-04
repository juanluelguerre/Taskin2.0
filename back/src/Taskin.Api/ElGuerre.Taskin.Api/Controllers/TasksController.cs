using ElGuerre.Taskin.Application.Tasks.Commands;
using ElGuerre.Taskin.Application.Tasks.Queries;
using ElGuerre.Taskin.Domain.Entities;
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

    // GET: api/Tasks?projectId={projectId}&page={page}&size={size}
    [HttpGet]
    public async Task<ActionResult<TaskListResponse>> GetTasks(
        [FromQuery] Guid? projectId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 25)
    {
        if (projectId.HasValue)
        {
            // Legacy endpoint - return tasks by project
            var query = new GetTasksByProjectIdQuery { ProjectId = projectId.Value };
            var tasks = await _mediator.Send(query);
            
            // Convert to paginated response for consistency
            var pagedTasks = tasks.Skip((page - 1) * size).Take(size).ToList();
            return Ok(new TaskListResponse
            {
                Items = pagedTasks,
                TotalCount = tasks.Count(),
                CurrentPage = page,
                PageSize = size
            });
        }

        // Return all tasks with pagination
        var searchQuery = new SearchTasksQuery
        {
            Page = page,
            Size = size
        };
        var result = await _mediator.Send(searchQuery);
        return Ok(result);
    }

    // POST: api/Tasks/search
    [HttpPost("search")]
    public async Task<ActionResult<TaskListResponse>> SearchTasks([FromBody] SearchTasksQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // GET: api/Tasks/stats
    [HttpGet("stats")]
    public async Task<ActionResult<TaskStatsResponse>> GetTaskStats([FromQuery] Guid? projectId)
    {
        var query = new GetTaskStatsQuery { ProjectId = projectId };
        var stats = await _mediator.Send(query);
        return Ok(stats);
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

    // POST: api/Tasks/{id}/duplicate
    [HttpPost("{id}/duplicate")]
    public async Task<ActionResult<Guid>> DuplicateTask(Guid id, [FromBody] DuplicateTaskRequest? request)
    {
        var command = new DuplicateTaskCommand 
        { 
            TaskId = id,
            NewTitle = request?.NewTitle
        };
        var newTaskId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTask), new { id = newTaskId }, newTaskId);
    }

    // POST: api/Tasks/{id}/toggle-completion
    [HttpPost("{id}/toggle-completion")]
    public async Task<ActionResult<TaskEntity>> ToggleTaskCompletion(Guid id)
    {
        var command = new ToggleTaskCompletionCommand { TaskId = id };
        var task = await _mediator.Send(command);
        return Ok(task);
    }

    // POST: api/Tasks/bulk-update-status
    [HttpPost("bulk-update-status")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusRequest request)
    {
        var command = new BulkUpdateTaskStatusCommand
        {
            TaskIds = request.TaskIds,
            Status = request.Status
        };
        await _mediator.Send(command);
        return NoContent();
    }
}

// DTOs for request bodies
public class DuplicateTaskRequest
{
    public string? NewTitle { get; set; }
}

public class BulkUpdateStatusRequest
{
    public List<Guid> TaskIds { get; set; } = [];
    public Domain.Entities.TaskStatus Status { get; set; }
}