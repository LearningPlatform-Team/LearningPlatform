using FluentValidation;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;

public class GetSubjectOfferingsQueryValidator
    : AbstractValidator<GetSubjectOfferingsQuery>
{
    public GetSubjectOfferingsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(20, 70);
    }
}
