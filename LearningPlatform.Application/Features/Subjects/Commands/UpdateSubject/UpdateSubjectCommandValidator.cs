using FluentValidation;

namespace LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;

public class UpdateSubjectCommandValidator
    : AbstractValidator<UpdateSubjectCommand>
{
    public UpdateSubjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Subject ID is required.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Subject code is required.")
            .MaximumLength(50)
            .WithMessage("Subject code cannot exceed 50 characters.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Subject name is required.")
            .MaximumLength(200)
            .WithMessage("Subject name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Subject description cannot exceed 1000 characters.");

        RuleFor(x => x.YearLevel)
            .IsInEnum()
            .WithMessage("Invalid year level.");
    }
}
