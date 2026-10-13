using FluentValidation;
namespace LearningPlatform.Application.Features.Events.GetEventById
{
    public sealed class GetEventByIdQueryValidator : AbstractValidator<GetEventByIdQuery>
    {
        public GetEventByIdQueryValidator()
        {
            RuleFor(query => query.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(query => query.RequestedByUserId)
                .NotEmpty()
                .WithMessage("The requesting user is required.");
        }
    }
}
