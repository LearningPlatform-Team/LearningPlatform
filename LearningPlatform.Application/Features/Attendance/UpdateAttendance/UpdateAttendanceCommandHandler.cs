using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LearningPlatform.Application.Features.Attendance.UpdateAttendance
{
    public sealed class UpdateAttendanceCommandHandler
        : IRequestHandler<UpdateAttendanceCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public UpdateAttendanceCommandHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(
            UpdateAttendanceCommand request,
            CancellationToken cancellationToken)
        {
            var attendanceRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(
                    x => x.Id == request.AttendanceRecordId,
                    cancellationToken);

            if (attendanceRecord is null)
            {
                return Result.Failure(
                    "Attendance record was not found.",
                    ErrorType.NotFound);
            }

            var updatingUserExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(
                    x => x.Id == request.UpdatedByUserId,
                    cancellationToken);

            if (!updatingUserExists)
            {
                return Result.Failure(
                    "The user performing the attendance update was not found.",
                    ErrorType.NotFound);
            }

            if (attendanceRecord.Status == request.Status)
            {
                return Result.Success();
            }

            attendanceRecord.Status = request.Status;
            attendanceRecord.MarkedByUserId = request.UpdatedByUserId;
            attendanceRecord.Source = AttendanceSource.Manual;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
