using FluentValidation;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;

public class AssignTaCommandValidator : AbstractValidator<AssignTaCommand>
{
    public AssignTaCommandValidator()
    {
        RuleFor(x => x.OfferingId)
            .NotEmpty();

        RuleFor(x => x.TaProfileId)
            .NotEmpty();
    }
}