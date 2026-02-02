using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}
