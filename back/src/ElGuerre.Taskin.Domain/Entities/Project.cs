using ElGuerre.Taskin.Domain.SeedWork;

namespace ElGuerre.Taskin.Domain.Entities;

public sealed class Project : TrackedEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateTime? DueDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? BackgroundColor { get; set; }
    public ICollection<Task> Tasks { get; init; } = new List<Task>();
}

public enum ProjectStatus
{
    Active = 0,
    Completed = 1,
    OnHold = 2
}