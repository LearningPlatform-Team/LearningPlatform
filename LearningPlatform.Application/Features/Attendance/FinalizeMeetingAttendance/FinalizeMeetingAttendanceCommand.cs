using LearningPlatform.Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Attendance.FinalizeMeetingAttendance
{

    public sealed record FinalizeMeetingAttendanceCommand(
        Guid ClassSessionId,
        DateTime FinalizedAtUtc
        ) : IRequest<Result>;
}
