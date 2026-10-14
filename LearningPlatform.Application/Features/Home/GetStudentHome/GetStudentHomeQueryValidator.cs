using FluentValidation;
namespace LearningPlatform.Application.Features.Home.GetStudentHome
{
    public sealed class GetStudentHomeQueryValidator : AbstractValidator<GetStudentHomeQuery>
    {
        public GetStudentHomeQueryValidator()
        {
            RuleFor(query => query.StudentId)
                .NotEmpty()
                .WithMessage("Student ID is required.");

            RuleFor(query => query.ItemsCount)
                .InclusiveBetween(1, 10)
                .WithMessage("Items count must be between 1 and 10.");
        }
    }
}
