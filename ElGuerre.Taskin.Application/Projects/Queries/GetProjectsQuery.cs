using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectsQuery : IRequest<List<Project>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
