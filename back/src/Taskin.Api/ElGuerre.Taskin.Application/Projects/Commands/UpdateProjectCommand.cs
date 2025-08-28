using ElGuerre.Taskin.Domain.Entities;
using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class UpdateProjectCommand : IRequest
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTime? DueDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundColor { get; set; }
}