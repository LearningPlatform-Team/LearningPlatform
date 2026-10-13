using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LearningPlatform.Application.Features.Attendance.GetSessionAttendance
{

    public sealed class GetSessionAttendanceQueryHandler : IRequestHandler<GetSessionAttendanceQuery,
            ResultT<PaginatedList<SessionAttendanceDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetSessionAttendanceQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ResultT<PaginatedList<SessionAttendanceDto>>> Handle(
            GetSessionAttendanceQuery request,
            CancellationToken cancellationToken)
        {
            var session = await _context.ClassSessions
                .AsNoTracking()
                .Where(x => x.Id == request.ClassSessionId)
                .Select(x => new
                {
                    x.Id,
                    x.SubjectOfferingId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return ResultT<PaginatedList<SessionAttendanceDto>>.Failure(
                    "Class session was not found.",
                    ErrorType.NotFound);
            }

            var query = _context.Enrollments
                .AsNoTracking()
                .Where(enrollment =>
                    enrollment.OfferingId == session.SubjectOfferingId &&
                    enrollment.Status == EnrollmentStatus.Enrolled)
                .Select(enrollment => new
                {
                    enrollment.StudentId,

                    StudentName = enrollment.Student.User.FullName,

                    Attendance = enrollment.AttendanceRecords
                        .Where(attendance =>
                            attendance.ClassSessionId == request.ClassSessionId)
                        .Select(attendance => new
                        {
                            attendance.Id,
                            attendance.Status,
                            attendance.MarkedAt
                        })
                        .FirstOrDefault()
                })
                .Select(item => new SessionAttendanceDto
                {
                    AttendanceRecordId = item.Attendance != null
                        ? item.Attendance.Id
                        : null,
                    StudentId = item.StudentId,
                    StudentName = item.StudentName,
                    Status = item.Attendance != null
                        ? item.Attendance.Status
                        : null,
                    MarkedAt = item.Attendance != null
                        ? item.Attendance.MarkedAt
                        : null
                });

            var totalCount = await query.CountAsync(
                cancellationToken);

            var items = await query
                .OrderBy(x => x.StudentName)
                .ThenBy(x => x.StudentId)
                .Skip(
                    (request.PageNumber - 1) *
                    request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginatedResult =
                new PaginatedList<SessionAttendanceDto>(
                    items,
                    totalCount,
                    request.PageNumber,
                    request.PageSize);

            return ResultT<
                PaginatedList<SessionAttendanceDto>>
                .Success(paginatedResult);
        }
    }
}
