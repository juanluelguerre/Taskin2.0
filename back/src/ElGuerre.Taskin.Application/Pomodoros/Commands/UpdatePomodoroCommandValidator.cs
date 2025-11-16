using FluentValidation;

namespace ElGuerre.Taskin.Application.Pomodoros.Commands;

public class UpdatePomodoroCommandValidator : AbstractValidator<UpdatePomodoroCommand>
{
    public UpdatePomodoroCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        When(x => x.StartTime.HasValue, () =>
        {
            RuleFor(x => x.StartTime)
                .NotEmpty()
                .WithMessage("StartTime is required when provided");
        });

        When(x => x.DurationInMinutes.HasValue, () =>
        {
            RuleFor(x => x.DurationInMinutes.Value)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0 minutes")
                .LessThanOrEqualTo(480) // 8 hours max
                .WithMessage("Pomodoro session cannot exceed 8 hours (480 minutes)");
        });
    }
}