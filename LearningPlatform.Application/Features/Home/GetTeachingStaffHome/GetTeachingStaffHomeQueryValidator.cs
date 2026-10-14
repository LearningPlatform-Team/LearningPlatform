using FluentValidation;
namespace LearningPlatform.Application.Features.Home.GetTeachingStaffHome
{
    public sealed class GetTeachingStaffHomeQueryValidator : AbstractValidator<GetTeachingStaffHomeQuery>
    {
        public GetTeachingStaffHomeQueryValidator()
        {
            RuleFor(query => query.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");

            RuleFor(query => query)
                .Must(query =>
                    query.IsDoctor ||
                    query.IsTA)
                .WithMessage("The user must be a doctor or teaching assistant.");

            RuleFor(query => query.ItemsCount)
                .InclusiveBetween(1, 10)
                .WithMessage("Items count must be between 1 and 10.");
        }
    }
}
