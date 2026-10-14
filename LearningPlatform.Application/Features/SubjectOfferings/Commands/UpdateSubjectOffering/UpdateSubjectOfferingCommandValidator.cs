using FluentValidation;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;

public class UpdateSubjectOfferingCommandValidator
    : AbstractValidator<UpdateSubjectOfferingCommand>
{
    public UpdateSubjectOfferingCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Subject offering ID is required.");

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