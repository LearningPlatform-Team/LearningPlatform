using LearningPlatform.Api.Extensions;
using LearningPlatform.Application.Features.Attendance.GetAttendanceSummary;
using LearningPlatform.Application.Features.Attendance.GetSessionAttendance;
using LearningPlatform.Application.Features.Attendance.GetStudentAttendance;
using LearningPlatform.Application.Features.Attendance.MarkManualAttendance;
using LearningPlatform.Application.Features.Attendance.UpdateAttendance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Student Attendance Management | تسجيل ومتابعة الحضور والغياب
    /// </summary>
    /// <remarks>
    /// Manages session attendance tracking, QR-code/manual attendance marking by Doctors &amp; TAs, and personal attendance reports for Students.
    /// إدارة رصد الحضور والغياب للمحاضرات، تسجيل الغياب يدوياً، واستعلام الطالب عن نسبة حضوره.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class AttendanceController : AppControllerBase
    {
        private readonly ISender _sender;

        public AttendanceController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Update individual attendance status | تعديل حالة حضور طالب في محاضرة
        /// </summary>
        /// <remarks>Roles: Doctor, TA, Admin.</remarks>
        /// <param name="attendanceRecordId">Attendance record GUID.</param>
        /// <param name="request">New attendance status (Present, Absent, Late, Excused).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Attendance updated successfully.</response>
        /// <response code="400">Invalid status.</response>
        /// <response code="403">Forbidden.</response>
        [HttpPut("update/{attendanceRecordId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAttendance(
            Guid attendanceRecordId,
            [FromBody] UpdateAttendanceRequestDto request,
            CancellationToken cancellationToken)
        {
            var currentUserId = User.GetUserId();
            var command = new UpdateAttendanceCommand(attendanceRecordId, request.Status, currentUserId);
            var result = await _sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get all students attendance for a class session | كشف حضور جلسة/محاضرة معينة
        /// </summary>
        /// <remarks>Roles: Doctor, TA, Admin.</remarks>
        /// <param name="classSessionId">Class session GUID.</param>
        /// <param name="pageNumber">Page index (default 1).</param>
        /// <param name="pageSize">Page size (default 20).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns list of students and their attendance status for this session.</response>
        [HttpGet("sessions/{classSessionId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSessionAttendance(
            Guid classSessionId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetSessionAttendanceQuery(classSessionId, pageNumber, pageSize);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get student attendance history | سجل حضور طالب معين
        /// </summary>
        /// <remarks>Roles: Doctor, TA, Admin.</remarks>
        /// <param name="studentId">Student user GUID.</param>
        /// <param name="pageNumber">Page index (default 1).</param>
        /// <param name="pageSize">Page size (default 20).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns attendance history records for the student.</response>
        [HttpGet("students/{studentId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudentAttendance(
            Guid studentId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetStudentAttendanceQuery(studentId, pageNumber, pageSize);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get My Attendance (Current Student) | استعلام الطالب عن سجل حضوره الشخصي
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        /// Returns all attendance records and sessions for the authenticated student.
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint في شاشة "الحضور والغياب" للطالب لعرض سجل المحاضرات التي حضرها أو غاب عنها.
        /// </remarks>
        /// <param name="pageNumber">Page index (default 1).</param>
        /// <param name="pageSize">Page size (default 20).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns paginated attendance records of the student.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyAttendance(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var studentId = User.GetUserId();
            var query = new GetStudentAttendanceQuery(studentId, pageNumber, pageSize);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get Attendance Summary for an Enrollment | ملخص ونسبة حضور الطالب في مقرر
        /// </summary>
        /// <remarks>Roles: Doctor, TA, Admin.</remarks>
        /// <param name="enrollmentId">Enrollment GUID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns attendance percentage, present count, absent count, and warning status.</response>
        [HttpGet("enrollments/{enrollmentId:guid}/summary")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAttendanceSummary(Guid enrollmentId, CancellationToken cancellationToken)
        {
            var query = new GetAttendanceSummaryQuery(enrollmentId);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Mark manual attendance for a session | رصد الحضور يدوياً لقائمة طلاب في جلسة
        /// </summary>
        /// <remarks>
        /// **Roles: Admin, Doctor, TA.**
        /// Allows teaching staff to mark multiple students as Present/Absent in batch for a session.
        /// </remarks>
        /// <param name="classSessionId">Class session GUID.</param>
        /// <param name="request">List of student enrollment IDs and their attendance status.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Batch attendance saved successfully.</response>
        [HttpPut("sessions/{classSessionId:guid}/manual")]
        [Authorize(Roles = "Admin,Doctor,TA")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkManualAttendance(
            Guid classSessionId,
            [FromBody] MarkManualAttendanceRequestDto request,
            CancellationToken cancellationToken)
        {
            var attendanceItems = request.Students
                .Select(item => new ManualAttendanceItem(item.EnrollmentId, item.Status))
                .ToList();
            var command = new MarkManualAttendanceCommand(
                classSessionId,
                attendanceItems,
                User.GetUserId(),
                User.IsAdmin(),
                User.IsDoctor(),
                User.IsTA());
            var result = await _sender.Send(command, cancellationToken);
            return HandleResult(result);
        }
    }
}
