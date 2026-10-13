using FluentValidation;

namespace LearningPlatform.Application.Features.Meetings.JoinMeeting
{
    public class JoinMeetingValidator : AbstractValidator<JoinMeetingCommand>
    {
        public JoinMeetingValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.JoinCode) || x.MeetingId.HasValue)
                .WithMessage("Either JoinCode or MeetingId must be provided to join a meeting.");
        }
    }
}
