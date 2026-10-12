using FluentValidation;
namespace LearningPlatform.Application.Features.EventReminders.GetEventReminders
{
    public sealed class GetEventRemindersQueryValidator : AbstractValidator<GetEventRemindersQuery>
    {
        private const int MaximumPageSize = 100;
        public GetEventRemindersQueryValidator()
        {
            RuleFor(query => query.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(query => query.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than zero.");

            RuleFor(query => query.PageSize)
                .InclusiveBetween(1, MaximumPageSize)
                .WithMessage($"Page size must be between 1 and {MaximumPageSize}.");
        }
    }
}
