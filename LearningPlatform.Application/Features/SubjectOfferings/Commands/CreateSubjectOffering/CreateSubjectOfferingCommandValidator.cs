using FluentValidation;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.CreateSubjectOffering;

public class CreateSubjectOfferingCommandValidator
    : AbstractValidator<CreateSubjectOfferingCommand>
{
    public CreateSubjectOfferingCommandValidator()
    {
        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage("Subject ID is required.");

        RuleFor(x => x.TermId)
            .NotEmpty()
            .WithMessage("Academic term ID is required.");

        RuleFor(x => x.DoctorProfileId)
            .NotEmpty()
            .WithMessage("Doctor profile ID is required.");
    }
}