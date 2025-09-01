using ElGuerre.Taskin.Application.Projects.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectsQuery : IRequest<CollectionResponse<ProjectListDto>>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? Search { get; set; }
    public string? Status { get; set; }
    public string? Sort { get; set; }
    public string? Order { get; set; }
}
