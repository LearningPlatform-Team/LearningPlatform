using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.Note.Command.CreateNote
{
    public class CreateNoteCommandValidator
    : AbstractValidator<CreateNoteCommand>
    {
        public CreateNoteCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Note title is required.")
                .MaximumLength(200)
                .WithMessage("Note title cannot exceed 200 characters.");

            RuleFor(x => x.Content)
                .MaximumLength(5000)
                .WithMessage("Note content cannot exceed 5000 characters.");

            RuleFor(x => x.Color)
                .IsInEnum()
                .WithMessage("Invalid note color.");
        }
    }
}
