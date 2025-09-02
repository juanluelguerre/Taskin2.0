using MediatR;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class DuplicateTaskCommand : IRequest<Guid>
{
    public Guid TaskId { get; set; }
    public string? NewTitle { get; set; } // Optional custom title for the duplicate
}