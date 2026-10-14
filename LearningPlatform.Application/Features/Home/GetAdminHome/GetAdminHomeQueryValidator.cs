using FluentValidation;
namespace LearningPlatform.Application.Features.Home.GetAdminHome;

public sealed class GetAdminHomeQueryValidator : AbstractValidator<GetAdminHomeQuery>
{
    public GetAdminHomeQueryValidator()
    {
        RuleFor(query => query.AdminId)
            .NotEmpty()
            .WithMessage("Admin ID is required.");

        RuleFor(query => query.IsAdmin)
            .Equal(true)
            .WithMessage("Only administrators can access the admin home.");

        RuleFor(query => query.ItemsCount)
            .InclusiveBetween(1, 10)
            .WithMessage("Items count must be between 1 and 10.");
    }
}