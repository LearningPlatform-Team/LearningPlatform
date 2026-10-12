using LearningPlatform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Attendance.GetStudentAttendance
{
    public sealed class StudentAttendanceDto
    {
        public Guid AttendanceRecordId { get; set; }
        public Guid ClassSessionId { get; set; }
        public Guid SubjectOfferingId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public DateTime? MarkedAt { get; set; }
    }
}
