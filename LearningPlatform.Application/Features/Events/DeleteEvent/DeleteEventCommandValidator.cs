using FluentValidation;
namespace LearningPlatform.Application.Features.Events.DeleteEvent
{
    public sealed class DeleteEventCommandValidator : AbstractValidator<DeleteEventCommand>
    {
        public DeleteEventCommandValidator()
        {
            RuleFor(command => command.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required.");

            RuleFor(command => command.DeletedByUserId)
                .NotEmpty()
                .WithMessage("The user performing the delete operation is required.");

            RuleFor(command => command)
                .Must(command => command.IsAdmin || command.IsDoctor || command.IsTA)
                .WithMessage("You are not allowed to delete events.");
        }
    }
}
