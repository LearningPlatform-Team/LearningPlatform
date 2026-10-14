using LearningPlatform.Api.Extensions;
using LearningPlatform.Application.Features.Attendance.GetAttendanceSummary;
using LearningPlatform.Application.Features.Attendance.GetSessionAttendance;
using LearningPlatform.Application.Features.Attendance.GetStudentAttendance;
using LearningPlatform.Application.Features.Attendance.MarkManualAttendance;
using LearningPlatform.Application.Features.Attendance.UpdateAttendance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AttendanceController : AppControllerBase
    {
        private readonly ISender _sender;


        public AttendanceController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPut("update/{attendanceRecordId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        public async Task<IActionResult> UpdateAttendance( Guid attendanceRecordId,
            [FromBody] UpdateAttendanceRequestDto request,CancellationToken cancellationToken)
        {
            var currentUserId = User.GetUserId();
            var command = new UpdateAttendanceCommand(attendanceRecordId,request.Status,currentUserId);
            var result = await _sender.Send(
                command,
                cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("sessions/{classSessionId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        public async Task<IActionResult> GetSessionAttendance(Guid classSessionId,[FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,CancellationToken cancellationToken = default)
        {
            var query = new GetSessionAttendanceQuery(classSessionId,pageNumber,pageSize);
            var result = await _sender.Send(
                query,
                cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("students/{studentId:guid}")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        public async Task<IActionResult> GetStudentAttendance(Guid studentId, [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var query = new GetStudentAttendanceQuery(studentId,pageNumber,pageSize);
            var result = await _sender.Send(query,cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("me")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyAttendance([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,
                 CancellationToken cancellationToken = default)
        {
            var studentId = User.GetUserId();
            var query = new GetStudentAttendanceQuery(studentId,pageNumber,pageSize);
            var result = await _sender.Send(query,cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("enrollments/{enrollmentId:guid}/summary")]
        [Authorize(Roles = "Doctor,TA,Admin")]
        public async Task<IActionResult> GetAttendanceSummary(Guid enrollmentId, CancellationToken cancellationToken)
        {
            var query = new GetAttendanceSummaryQuery(enrollmentId);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }
        [HttpPut("sessions/{classSessionId:guid}/manual")]
        [Authorize(Roles = "Admin,Doctor,TA")]
        public async Task<IActionResult> MarkManualAttendance(Guid classSessionId,[FromBody] MarkManualAttendanceRequestDto request,
            CancellationToken cancellationToken)
        {
            var attendanceItems = request.Students
                .Select(item => new ManualAttendanceItem(item.EnrollmentId,item.Status))
                .ToList();
            var command = new MarkManualAttendanceCommand(
                classSessionId,
                attendanceItems,
                User.GetUserId(),
                User.IsAdmin(),
                User.IsDoctor(),
                User.IsTA());
            var result = await _sender.Send(command,cancellationToken);
            return HandleResult(result);
        }
    }
}

