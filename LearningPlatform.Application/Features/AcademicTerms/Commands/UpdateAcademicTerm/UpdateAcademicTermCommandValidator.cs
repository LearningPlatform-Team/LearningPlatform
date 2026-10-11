using FluentValidation;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.UpdateAcademicTerm;

public class UpdateAcademicTermCommandValidator
    : AbstractValidator<UpdateAcademicTermCommand>
{
    public UpdateAcademicTermCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Academic term ID is required.");

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .MaximumLength(20)
            .WithMessage("Academic year cannot exceed 20 characters.");

        RuleFor(x => x.TermNumber)
            .InclusiveBetween(1, 2)
            .WithMessage("Term number must be either 1 or 2.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after the start date.");
    }
}