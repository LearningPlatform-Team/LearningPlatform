using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Attendance.GetSessionAttendance
{

    public sealed class GetSessionAttendanceQueryValidator : AbstractValidator<GetSessionAttendanceQuery>
    {
        private const int MaximumPageSize = 100;

        public GetSessionAttendanceQueryValidator()
        {
            RuleFor(x => x.ClassSessionId)
                .NotEmpty()
                .WithMessage("Class session ID is required.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than zero.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, MaximumPageSize)
                .WithMessage(
                    $"Page size must be between 1 and {MaximumPageSize}.");
        }
    }
}
