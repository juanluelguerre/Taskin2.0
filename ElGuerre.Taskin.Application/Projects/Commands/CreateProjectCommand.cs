using MediatR;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class CreateProjectCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundColor { get; set; }
}
