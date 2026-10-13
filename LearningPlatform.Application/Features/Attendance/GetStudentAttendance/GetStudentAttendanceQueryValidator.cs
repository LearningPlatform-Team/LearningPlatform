using FluentValidation;
using LearningPlatform.Application.Features.Attendance.GetStudentAttendance;

namespace LearningPlatform.Application.Attendance.GetStudentAttendance;

public sealed class GetStudentAttendanceQueryValidator : AbstractValidator<GetStudentAttendanceQuery>
{
    private const int MaximumPageSize = 100;

    public GetStudentAttendanceQueryValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .WithMessage("Student ID is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaximumPageSize)
            .WithMessage(
                $"Page size must be between 1 and {MaximumPageSize}.");
    }
}