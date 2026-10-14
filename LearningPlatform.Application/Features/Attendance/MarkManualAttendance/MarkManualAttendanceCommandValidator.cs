using FluentValidation;

namespace LearningPlatform.Application.Features.Attendance.MarkManualAttendance;

public sealed class MarkManualAttendanceCommandValidator : AbstractValidator<MarkManualAttendanceCommand>
{
    public MarkManualAttendanceCommandValidator()
    {
        RuleFor(command => command.ClassSessionId)
            .NotEmpty()
            .WithMessage("Class session ID is required.");

        RuleFor(command => command.MarkedByUserId)
            .NotEmpty()
            .WithMessage("The user marking attendance is required.");

        RuleFor(command => command.Students)
            .NotEmpty()
            .WithMessage("At least one attendance item is required.");

        RuleFor(command => command.Students)
            .Must(students => students
                    .Select(item => item.EnrollmentId)
                    .Distinct()
                    .Count() == students.Count)
            .WithMessage("The attendance list contains duplicate enrollments.");

        RuleForEach(command => command.Students)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.EnrollmentId)
                    .NotEmpty()
                    .WithMessage("Enrollment ID is required.");

                item.RuleFor(x => x.Status)
                    .IsInEnum()
                    .WithMessage("Invalid attendance status.");
            });

        RuleFor(command => command)
            .Must(command => command.IsAdmin ||command.IsDoctor ||command.IsTA)
            .WithMessage("You are not allowed to mark attendance manually.");
    }
}