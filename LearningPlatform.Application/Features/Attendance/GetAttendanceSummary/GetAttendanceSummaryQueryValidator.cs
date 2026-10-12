using FluentValidation;
namespace LearningPlatform.Application.Features.Attendance.GetAttendanceSummary
{
    public sealed class GetAttendanceSummaryQueryValidator : AbstractValidator<GetAttendanceSummaryQuery>
    {
        public GetAttendanceSummaryQueryValidator()
        {
            RuleFor(x => x.EnrollmentId)
                .NotEmpty()
                .WithMessage("Enrollment ID is required.");
        }
    }
}
