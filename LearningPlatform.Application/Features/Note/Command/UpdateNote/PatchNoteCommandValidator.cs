using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.Note.Command.UpdateNote
{
    public class PatchNoteCommandValidator : AbstractValidator<PatchNoteCommand>
    {
        public PatchNoteCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Note id is required.");

            RuleFor(x => x)
                .Must(x => x.Title is not null || x.Content is not null || x.Color.HasValue)
                .WithMessage("At least one field (title, content, color) must be provided.");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Note title cannot be empty.")
                .MaximumLength(200)
                .WithMessage("Note title cannot exceed 200 characters.")
                .When(x => x.Title is not null);

            RuleFor(x => x.Content)
                .MaximumLength(5000)
                .WithMessage("Note content cannot exceed 5000 characters.")
                .When(x => x.Content is not null);

            RuleFor(x => x.Color)
                .IsInEnum()
                .WithMessage("Invalid note color.")
                .When(x => x.Color.HasValue);
        }
    }
}
