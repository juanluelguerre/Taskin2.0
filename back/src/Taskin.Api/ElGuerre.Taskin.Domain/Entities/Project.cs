using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Project : TrackedEntity
{
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundColor { get; set; }
    public ICollection<Task> Tasks { get; init; } = new List<Task>();
}