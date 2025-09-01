using FluentValidation;

namespace ElGuerre.Taskin.Application.Tasks.Commands;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.ProjectId)
            .NotEmpty()
            .WithMessage("ProjectId is required");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid TaskStatus");

        When(x => x.Deadline.HasValue, () =>
        {
            RuleFor(x => x.Deadline.Value)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Deadline must be in the future");
        });
    }
}