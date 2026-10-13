using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Attendance.UpdateAttendance
{
    public sealed class UpdateAttendanceCommandValidator
        : AbstractValidator<UpdateAttendanceCommand>
    {
        public UpdateAttendanceCommandValidator()
        {
            RuleFor(x => x.AttendanceRecordId)
                .NotEmpty()
                .WithMessage("Attendance record ID is required.");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid attendance status.");

            RuleFor(x => x.UpdatedByUserId)
                .NotEmpty()
                .WithMessage("The user performing the update is required.");
        }
    }
}
