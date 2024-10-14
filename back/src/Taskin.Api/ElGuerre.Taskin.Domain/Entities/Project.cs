using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Project : TrackedEntity
{
    public required string Name { get; init; }
    public string? ImageUrl { get; init; }
    public string? BackgroundColor { get; init; }
    public ICollection<Task> Tasks { get; init; } = new List<Task>();
}