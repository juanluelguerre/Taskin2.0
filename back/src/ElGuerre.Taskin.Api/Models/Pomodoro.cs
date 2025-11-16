namespace ElGuerre.Taskin.Api.Models;

public class Pomodoro
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public int DurationInMinutes { get; set; }

    public Task Task { get; set; }
}
