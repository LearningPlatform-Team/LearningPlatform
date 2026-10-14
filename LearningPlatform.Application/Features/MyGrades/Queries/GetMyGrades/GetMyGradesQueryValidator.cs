using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades
{
    public class GetMyGradesQueryValidator : AbstractValidator<GetMyGradesQuery>
    {
        public GetMyGradesQueryValidator()
        {
            RuleFor(x => x.TermId)
                .NotEqual(Guid.Empty)
                .WithMessage("Term id is invalid.")
                .When(x => x.TermId.HasValue);
        }
    }
}
