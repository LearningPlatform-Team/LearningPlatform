using LearningPlatform.Application.Features.Subjects.Commands.CreateSubject;
using LearningPlatform.Application.Features.Subjects.Commands.DeleteSubject;
using LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;
using LearningPlatform.Application.Features.Subjects.Queries.GetMySubjects;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectBooks;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectLectures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Academic Subjects &amp; Course Materials | إدارة المواد الأكاديمية والمحتوى
/// </summary>
/// <remarks>
/// Allows querying subjects, student course enrollments, lectures list for a subject, and associated digital books.
/// إدارة المقررات الأكاديمية والمحاضرات والكتب التابعة لكل مقرر.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SubjectsController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Get subject details by ID | جلب تفاصيل مقرر بالمعرف
    /// </summary>
    /// <param name="id">Subject unique GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns subject details.</response>
    /// <response code="404">Subject not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get student's enrolled subjects | جلب قائمة المواد المسجلة للطالب الحالي
    /// </summary>
    /// <remarks>
    /// **Role: Student only.**
    /// Returns the list of subjects the authenticated student is currently enrolled in for active terms.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint لعرض قائمة المواد / المقررات المسجلة في الصفحة الرئيسية للدروس.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns array of enrolled subjects.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden — caller is not a Student.</response>
    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMySubjects(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMySubjectsQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Create new subject | إضافة مقرر أكاديمي جديد
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">Subject details (Name, Code, Credits, Department, Description).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Subject created successfully.</response>
    /// <response code="400">Validation failure or duplicate code.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update existing subject | تعديل بيانات مقرر أكاديمي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Subject ID in URL path.</param>
    /// <param name="command">Updated subject details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Subject updated successfully.</response>
    /// <response code="400">Route ID does not match command ID.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete subject | حذف مقرر أكاديمي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Subject ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Subject deleted successfully.</response>
    /// <response code="404">Subject not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteSubjectCommand(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all lectures for a subject | جلب جميع محاضرات المقرر الدراسي
    /// </summary>
    /// <remarks>
    /// **Role: Student only.**
    /// Returns video lectures, titles, order numbers, and duration for the specified subject.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint عند فتح صفحة تفاصيل مادة لعرض قائمة الفيديوهات والمحاضرات المسجلة.
    /// </remarks>
    /// <param name="subjectId">Subject ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of lectures for the subject.</response>
    /// <response code="404">Subject not found.</response>
    [HttpGet("{subjectId:guid}/lectures")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectLectures(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectLecturesQuery(subjectId),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all reference books for a subject | جلب الكتب والمراجع المرفقة بالمقرر
    /// </summary>
    /// <remarks>
    /// **Role: Student only.**
    /// Returns the collection of PDF books and textbook references associated with this subject.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint في تبويب "الكتب / المراجع" داخل شاشة تفاصيل المادة.
    /// </remarks>
    /// <param name="subjectId">Subject ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of books for the subject.</response>
    /// <response code="404">Subject not found.</response>
    [HttpGet("{subjectId:guid}/books")]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubjectBooks(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectBooksQuery(subjectId),
            cancellationToken);

        return HandleResult(result);
    }
}