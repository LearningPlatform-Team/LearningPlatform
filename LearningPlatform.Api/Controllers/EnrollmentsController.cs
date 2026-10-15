using LearningPlatform.Application.Features.Enrollments.Commands.CancelEnrollment;
using LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;
using LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollmentById;
using LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;
using LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Student Course Enrollments | تسجيل الطلاب في المواد والمقررات
/// </summary>
/// <remarks>
/// Handles student course enrollments, admin course assignment, and student academic history retrieval.
/// إدارة تسجيل الطلاب في المقررات الدراسية واستعراض السجل الأكاديمي السابق للطالب.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class EnrollmentsController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Enroll a student in a course offering | تسجيل طالب في مقرر دراسي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">StudentId and SubjectOfferingId.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Enrollment created successfully.</response>
    /// <response code="400">Student already enrolled or invalid offering.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Query all course enrollments | استعلام جميع التسجيلات
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="query">Filter by student, offering, or term.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of enrollments.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetEnrollmentsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get enrollment details by ID | تفاصيل تسجيل معين
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="enrollmentId">Enrollment GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns enrollment details.</response>
    /// <response code="404">Enrollment not found.</response>
    [HttpGet("{enrollmentId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetEnrollmentByIdQuery(enrollmentId),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Cancel a student enrollment | إلغاء تسجيل طالب في مقرر
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="enrollmentId">Enrollment GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Enrollment cancelled.</response>
    [HttpDelete("{enrollmentId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var command = new CancelEnrollmentCommand(enrollmentId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get my enrollment history | السجل الأكاديمي للمقررات المسجلة للطالب
    /// </summary>
    /// <remarks>
    /// **Role: Student only.**
    /// Returns the authenticated student's full historical enrollments grouped across past and current academic terms.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint في شاشة "السجل الأكاديمي" أو "المواد السابقة" للطالب.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns full enrollment history for the student.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-history")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyHistory(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMyEnrollmentHistoryQuery(),
            cancellationToken);

        return HandleResult(result);
    }
}