using FluentValidation;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class CreatePomodoroCommandValidator : AbstractValidator<CreatePomodoroCommand>
{
    public CreatePomodoroCommandValidator()
    {
        RuleFor(x => x.TaskId)
            .NotEmpty()
            .WithMessage("TaskId is required");

        RuleFor(x => x.StartTime)
            .NotEmpty()
            .WithMessage("StartTime is required");

        RuleFor(x => x.DurationInMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than 0 minutes")
            .LessThanOrEqualTo(480) // 8 hours max
            .WithMessage("Pomodoro session cannot exceed 8 hours (480 minutes)");
    }
}