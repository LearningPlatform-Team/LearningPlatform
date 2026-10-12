using FluentValidation;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Events.CreateEvent;

public sealed class CreateEventCommandValidator
    : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .WithMessage("Event title is required.")
            .MaximumLength(200)
            .WithMessage("Event title must not exceed 200 characters.");

        RuleFor(command => command.Description)
            .MaximumLength(2000)
            .WithMessage("Event description must not exceed 2000 characters.")
            .When(command =>!string.IsNullOrWhiteSpace(command.Description));

        RuleFor(command => command.Type)
            .IsInEnum()
            .WithMessage("Invalid event type.");

        RuleFor(command => command.StartsAtUtc)
            .NotEmpty()
            .WithMessage("Event start time is required.");

        RuleFor(command => command.EndsAtUtc)
            .GreaterThan(command => command.StartsAtUtc)
            .WithMessage("Event end time must be after the start time.")
            .When(command => command.EndsAtUtc.HasValue);

        RuleFor(command => command.Location)
            .MaximumLength(300)
            .WithMessage("Event location must not exceed 300 characters.")
            .When(command =>!string.IsNullOrWhiteSpace(command.Location));

        RuleFor(command => command.SubjectOfferingId)
            .NotEqual(Guid.Empty)
            .WithMessage("Subject offering ID is invalid.")
            .When(command =>command.SubjectOfferingId.HasValue);

        RuleFor(command => command.CreatedByUserId)
            .NotEmpty()
            .WithMessage("The event creator is required.");

        RuleFor(command => command)
            .Must(command =>
                command.IsAdmin ||
                command.IsDoctor ||
                command.IsTA)
            .WithMessage("You are not allowed to create events.");

        RuleFor(command => command.SubjectOfferingId)
            .NotNull()
            .WithMessage("Doctors and teaching assistants must link the event to a subject offering.")
            .When(command => !command.IsAdmin && (command.IsDoctor || command.IsTA));

        RuleFor(command => command.SubjectOfferingId)
            .Null()
            .WithMessage("General events cannot be linked to a subject offering.")
            .When(command => command.IsAdmin && !command.IsDoctor && !command.IsTA);
    }
}
