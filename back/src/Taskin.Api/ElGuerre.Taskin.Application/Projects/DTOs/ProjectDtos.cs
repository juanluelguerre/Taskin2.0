namespace ElGuerre.Taskin.Application.Projects.DTOs;

public record ProjectListDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    int Progress,
    int TotalTasks,
    int CompletedTasks,
    DateTime DueDate,
    string? ImageUrl,
    string? BackgroundColor,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ProjectDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    int Progress,
    int TotalTasks,
    int CompletedTasks,
    DateTime DueDate,
    string? ImageUrl,
    string? BackgroundColor,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<TaskSummaryDto> Tasks,
    List<TeamMemberDto> TeamMembers
);

public record TaskSummaryDto(
    Guid Id,
    string Title,
    string Status,
    string Priority
);

public record TeamMemberDto(
    Guid Id,
    string Name,
    string Email,
    string Initials,
    string? Avatar
);

public record CollectionResponse<T>(
    List<T> Data,
    int Total,
    int Page,
    int Size
);

public record ActionResponse(
    Guid Id,
    string Message,
    bool Success = true
);

public record ProjectStatsDto(
    int Total,
    int Active,
    int Completed,
    int OnHold
);