using FluentValidation;
namespace LearningPlatform.Application.Features.Events.GetUpcomingEvents
{
    public sealed class GetUpcomingEventsQueryValidator : AbstractValidator<GetUpcomingEventsQuery>
    {
        private const int MaximumTake = 20;

        public GetUpcomingEventsQueryValidator()
        {
            RuleFor(query => query.RequestedByUserId)
                .NotEmpty()
                .WithMessage("The requesting user is required.");

            RuleFor(query => query.Take)
                .InclusiveBetween(1, MaximumTake)
                .WithMessage($"Take must be between 1 and {MaximumTake}.");
        }
    }
}
