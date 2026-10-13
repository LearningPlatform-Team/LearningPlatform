using FluentValidation;
namespace LearningPlatform.Application.Features.Events.GetEvents
{

    public sealed class GetEventsQueryValidator : AbstractValidator<GetEventsQuery>
    {
        private const int MaximumPageSize = 100;

        public GetEventsQueryValidator()
        {
            RuleFor(query => query.RequestedByUserId)
                .NotEmpty()
                .WithMessage("The requesting user is required.");

            RuleFor(query => query.Type)
                .IsInEnum()
                .WithMessage("Invalid event type.")
                .When(query => query.Type.HasValue);

            RuleFor(query => query.SubjectOfferingId)
                .NotEqual(Guid.Empty)
                .WithMessage("Subject offering ID is invalid.")
                .When(query => query.SubjectOfferingId.HasValue);

            RuleFor(query => query.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than zero.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, MaximumPageSize)
                .WithMessage($"Page size must be between 1 and {MaximumPageSize}.");

            RuleFor(query => query.ToUtc)
                .GreaterThanOrEqualTo(query => query.FromUtc)
                .WithMessage("The end date must be greater than or equal to the start date.")
                .When(query => query.FromUtc.HasValue && query.ToUtc.HasValue);
        }
    }
}
