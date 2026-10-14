using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.MyLibrary.Quiers.GetMyLibrary
{
    public class GetMyLibraryQueryValidator : AbstractValidator<GetMyLibraryQuery>
    {
        public GetMyLibraryQueryValidator()
        {
            RuleFor(x => x.SubjectId)
                .NotEqual(Guid.Empty)
                .WithMessage("Subject id is invalid.")
                .When(x => x.SubjectId.HasValue);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage("Search text cannot exceed 100 characters.");

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50.");
        }
    }
}
