using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Attendance.RecordAttendance
{
    public sealed record RecordAttendanceCommand(
        Guid ClassSessionId,
        Guid StudentId,
        DateTime JoinedAtUtc
        ) : IRequest<ResultT<Guid>>;
}
