using FluentValidation;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(4000)
            .WithMessage("Notes cannot exceed 4000 characters");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid TaskStatus");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Priority must be a valid TaskPriority");

        When(x => x.Deadline.HasValue, () =>
        {
            RuleFor(x => x.Deadline!.Value)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Deadline must be in the future");
        });
    }
}
