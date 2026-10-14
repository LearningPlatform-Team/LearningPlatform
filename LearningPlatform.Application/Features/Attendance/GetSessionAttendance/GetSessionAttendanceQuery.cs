using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Attendance.GetSessionAttendance
{
    public sealed record GetSessionAttendanceQuery(
        Guid ClassSessionId,
        int PageNumber = 1,
        int PageSize = 20
    ) : IRequest<ResultT<PaginatedList<SessionAttendanceDto>>>;
}
