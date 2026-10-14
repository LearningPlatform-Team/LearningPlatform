using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.Note.Queries.GetAllNotesQuery
{
    public class GetAllNotesQueryValidator : AbstractValidator<GetAllNotesQuery>
    {
        public GetAllNotesQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");

            RuleFor(x => x.Search)
               .MaximumLength(100)
               .WithMessage("Search text cannot exceed 100 characters.");
        }
    }
}
