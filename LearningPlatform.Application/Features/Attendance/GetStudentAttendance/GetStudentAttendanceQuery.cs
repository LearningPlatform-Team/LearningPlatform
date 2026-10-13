using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Attendance.GetStudentAttendance
{
    public sealed record GetStudentAttendanceQuery(
        Guid StudentId,
        int PageNumber = 1,
        int PageSize = 20
        ) : IRequest<ResultT<PaginatedList<StudentAttendanceDto>>>;
}

