using FluentValidation;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;

public class CreateAcademicTermCommandValidator
    : AbstractValidator<CreateAcademicTermCommand>
{
    public CreateAcademicTermCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Academic term name is required.")
            .MaximumLength(100)
            .WithMessage("Academic term name cannot exceed 100 characters.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");
    }
}