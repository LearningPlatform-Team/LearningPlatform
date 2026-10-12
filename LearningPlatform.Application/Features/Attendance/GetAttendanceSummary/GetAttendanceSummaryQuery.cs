using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Attendance.GetAttendanceSummary
{

    public sealed record GetAttendanceSummaryQuery(
        Guid EnrollmentId
    ) : IRequest<ResultT<AttendanceSummaryDto>>;
}
