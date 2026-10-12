using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace LearningPlatform.Application.Features.Attendance.GetStudentAttendance
{

    public sealed class GetStudentAttendanceQueryHandler
        : IRequestHandler<
            GetStudentAttendanceQuery,
            ResultT<PaginatedList<StudentAttendanceDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetStudentAttendanceQueryHandler(
            IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ResultT<PaginatedList<StudentAttendanceDto>>> Handle(
            GetStudentAttendanceQuery request,
            CancellationToken cancellationToken)
        {
            var studentExists = await _context.StudentProfiles
                .AsNoTracking()
                .AnyAsync(student => student.UserId == request.StudentId,
                    cancellationToken);

            if (!studentExists)
            {
                return ResultT<PaginatedList<StudentAttendanceDto>>
                    .Failure("Student was not found.", ErrorType.NotFound);
            }
            var query = _context.AttendanceRecords
                .AsNoTracking()
                .Where(attendance =>
                    attendance.Enrollment.StudentId == request.StudentId)
                .Select(attendance => new StudentAttendanceDto
                {
                    AttendanceRecordId = attendance.Id,
                    ClassSessionId = attendance.ClassSessionId,
                    SubjectOfferingId =
                        attendance.ClassSession.SubjectOfferingId,
                    SubjectName = attendance.ClassSession
                            .SubjectOffering
                            .Subject.Name,
                    SessionDate = attendance.ClassSession.SessionDate,
                    StartTime = attendance.ClassSession.StartTime,
                    EndTime = attendance.ClassSession.EndTime,
                    Status = attendance.Status,
                    MarkedAt = attendance.MarkedAt
                });

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(item => item.SessionDate)
                .ThenByDescending(item => item.StartTime)
                .ThenBy(item => item.AttendanceRecordId)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var paginatedList =
                new PaginatedList<StudentAttendanceDto>(items, totalCount, request.PageNumber, request.PageSize);
            return ResultT<PaginatedList<StudentAttendanceDto>>
                .Success(paginatedList);
        }
    }
}
