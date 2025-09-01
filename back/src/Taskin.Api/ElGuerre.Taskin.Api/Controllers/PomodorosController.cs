using ElGuerre.Taskin.Application.Pomodoros.Commands;
using ElGuerre.Taskin.Application.Pomodoros.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PomodoroEntity = ElGuerre.Taskin.Domain.Entities.Pomodoro;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PomodorosController(IMediator mediator) : ControllerBase
{
    // GET: api/Pomodoros/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<PomodoroEntity>> GetPomodoro(Guid id)
    {
        var query = new GetPomodoroByIdQuery { Id = id };
        var pomodoro = await mediator.Send(query);

        if (pomodoro is null)
        {
            return NotFound();
        }

        return Ok(pomodoro);
    }

    // GET: api/Pomodoros?taskId={taskId}
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PomodoroEntity>>> GetPomodoros(
        [FromQuery] Guid taskId)
    {
        var query = new GetPomodorosByTaskIdQuery { TaskId = taskId };
        var pomodoros = await mediator.Send(query);

        return Ok(pomodoros);
    }

    // POST: api/Pomodoros
    [HttpPost]
    public async Task<ActionResult<Guid>> CreatePomodoro([FromBody] CreatePomodoroCommand command)
    {
        var pomodoroId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetPomodoro), new { id = pomodoroId }, pomodoroId);
    }

    // PUT: api/Pomodoros/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePomodoro(Guid id,
        [FromBody] UpdatePomodoroCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await mediator.Send(command);
        return NoContent();
    }

    // DELETE: api/Pomodoros/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePomodoro(Guid id)
    {
        var command = new DeletePomodoroCommand { Id = id };
        await mediator.Send(command);
        return NoContent();
    }
}