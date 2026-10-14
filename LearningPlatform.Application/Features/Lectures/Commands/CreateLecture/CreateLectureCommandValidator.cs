using FluentValidation;

namespace LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;

public sealed class CreateLectureCommandValidator
    : AbstractValidator<CreateLectureCommand>
{
    public CreateLectureCommandValidator()
    {
        RuleFor(x => x.SubjectOfferingId)
            .NotEmpty()
            .WithMessage("Subject offering ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage("Duration must be greater than zero.");

        RuleFor(x => x.VideoProvider)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.VideoUrl)
            .NotEmpty()
            .Must(BeAValidUrl)
            .WithMessage("Video URL must be a valid absolute URL.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0);
    }

    private static bool BeAValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
        (uri.Scheme == Uri.UriSchemeHttps ||
         uri.Scheme == Uri.UriSchemeHttp);
}