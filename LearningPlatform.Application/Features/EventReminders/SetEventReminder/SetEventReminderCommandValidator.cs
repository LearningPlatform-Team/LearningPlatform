using FluentValidation;
using LearningPlatform.Application.Features.EventReminders.CreateEventReminder;

namespace LearningPlatform.Application.Features.EventReminders.SetEventReminder;

public sealed class SetEventReminderCommandValidator : AbstractValidator<SetEventReminderCommand>
{
    private const int MaximumMinutesBefore = 43_200;
    public SetEventReminderCommandValidator()
    {
        RuleFor(command => command.EventId)
            .NotEmpty()
            .WithMessage("Event ID is required.");

        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");

        RuleFor(command => command.MinutesBefore)
            .InclusiveBetween(1, MaximumMinutesBefore)
            .WithMessage($"Reminder minutes must be between 1 and {MaximumMinutesBefore}.");
    }
}