using ElGuerre.Taskin.Application.Tasks.DTOs;
using ElGuerre.Taskin.Application.Tasks.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ElGuerre.Taskin.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
    {
        var query = new GetDashboardStatsQuery();
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("recent-activity")]
    public async Task<ActionResult<List<RecentActivityDto>>> GetRecentActivity([FromQuery] int limit = 10)
    {
        var query = new GetRecentActivityQuery { Limit = limit };
        var result = await mediator.Send(query);
        return Ok(result);
    }
}
