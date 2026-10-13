using LearningPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Attendance.GetSessionAttendance
{
    public sealed class SessionAttendanceDto
    {
        public Guid? AttendanceRecordId { get; set; }

        public Guid StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public AttendanceStatus? Status { get; set; }

        public DateTime? MarkedAt { get; set; }
    }
}
