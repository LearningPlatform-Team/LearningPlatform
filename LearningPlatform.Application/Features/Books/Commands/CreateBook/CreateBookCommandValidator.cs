using FluentValidation;

namespace LearningPlatform.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandValidator
    : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.FileStorageKey)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.SubjectId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("Subject ID must be a valid GUID.");
    }
}