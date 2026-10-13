using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.Mylist.Command.CreateList
{
    public class AddToMyListCommandValidator : AbstractValidator<AddToMyListCommand>
    {
        public AddToMyListCommandValidator()
        {
            RuleFor(x => x.LectureId)
                .NotEmpty()
                .WithMessage("Lecture id is required.");
        }
    }
}
