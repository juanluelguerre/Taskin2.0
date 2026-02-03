using ElGuerre.Taskin.Application.Tasks.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Queries;

public class GetRecentActivityQuery : IRequest<List<RecentActivityDto>>
{
    public int Limit { get; init; } = 10;
}
