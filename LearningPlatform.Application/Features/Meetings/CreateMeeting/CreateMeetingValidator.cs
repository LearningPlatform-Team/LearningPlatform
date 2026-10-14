using FluentValidation;

namespace LearningPlatform.Application.Features.Meetings.CreateMeeting
{
    public class CreateMeetingValidator : AbstractValidator<CreateMeetingCommand>
    {
        public CreateMeetingValidator()
        {
            RuleFor(x => x.SubjectOfferingId)
                .NotEmpty()
                .WithMessage("Subject Offering is required.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(256)
                .WithMessage("Meeting title is required.");

            RuleFor(x => x.MinutesDuration)
                .GreaterThan(0)
                .LessThanOrEqualTo(480)
                .WithMessage("Duration must be between 1 and 480 minutes.");

            RuleFor(x => x.ScheduledAt)
                .GreaterThan(DateTime.UtcNow)
                .When(x => !x.IsInstant && x.ScheduledAt.HasValue)
                .WithMessage("Scheduled time must be in the future.");
        }
    }
}
