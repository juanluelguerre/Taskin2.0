using ElGuerre.Taskin.Application.Projects.DTOs;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectByIdQuery : IRequest<ProjectDetailsDto>
{
    public Guid Id { get; set; }
}