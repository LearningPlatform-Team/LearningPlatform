using FluentValidation;

namespace LearningPlatform.Application.Features.Lectures.Commands.UpdateLecture;

public sealed class UpdateLectureCommandValidator
    : AbstractValidator<UpdateLectureCommand>
{
    public UpdateLectureCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.SubjectOfferingId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0);

        RuleFor(x => x.VideoProvider)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.VideoUrl)
            .NotEmpty()
            .Must(url =>
                Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttps ||
                 uri.Scheme == Uri.UriSchemeHttp))
            .WithMessage("Video URL must be a valid absolute URL.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}