using ElGuerre.Taskin.Application.Projects.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectStatsQuery : IRequest<ProjectStatsDto>
{
}