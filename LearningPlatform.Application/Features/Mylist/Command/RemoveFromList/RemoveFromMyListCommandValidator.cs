using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.Mylist.Command.RemoveFromList
{
    public class RemoveFromMyListCommandValidator : AbstractValidator<RemoveFromMyListCommand>
    {
        public RemoveFromMyListCommandValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty()
                .WithMessage("Lecture id is required.");
        }
    }
}
