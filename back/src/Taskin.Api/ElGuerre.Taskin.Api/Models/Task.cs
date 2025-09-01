using ElGuerre.Taskin.Domain.Entities;

namespace ElGuerre.Taskin.Api.Models;

public class Task
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public string Status { get; set; } // "todo", "in-progress", "done"
    public DateTime? Deadline { get; set; }
    public List<Pomodoro> Pomodoros { get; set; }

    public Project Project { get; set; }
}
