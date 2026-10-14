using FluentValidation;
namespace LearningPlatform.Application.Features.EventReminders.DeleteEventReminder
{
    public sealed class DeleteEventReminderCommandValidator : AbstractValidator<DeleteEventReminderCommand>
    {
        public DeleteEventReminderCommandValidator()
        {
            RuleFor(command => command.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(command => command.UserId)
                .NotEmpty()
                .WithMessage("User ID is required.");
        }
    }
}
