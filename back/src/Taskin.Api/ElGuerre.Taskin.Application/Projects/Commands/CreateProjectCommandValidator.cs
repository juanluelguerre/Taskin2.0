using FluentValidation;

namespace ElGuerre.Taskin.Application.Projects.Commands;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Project name required.")
            .MaximumLength(100).WithMessage("The name cannot exceed 100 characters.");
    }
}
