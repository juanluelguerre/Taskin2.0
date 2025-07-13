using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Queries;

public class GetProjectByIdQuery : IRequest<Project>
{
    public Guid Id { get; set; }
}