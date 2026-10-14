using FluentValidation;
namespace LearningPlatform.Application.Features.Attendance.RecordAttendance
{
    public class RecordAttendanceCommandValidator : AbstractValidator<RecordAttendanceCommand>
    {
        public RecordAttendanceCommandValidator()
        {

            RuleFor(x => x.ClassSessionId)
                .NotEmpty()
                .WithMessage("Class session ID is required.");

            RuleFor(x => x.StudentId)
                .NotEmpty()
                .WithMessage("Student ID is required.");

            RuleFor(x => x.JoinedAtUtc)
                .NotEmpty()
                .WithMessage("Join time is required.");
        }
    }
}
