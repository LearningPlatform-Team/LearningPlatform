using LearningPlatform.Api.Controllers;
using LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;
using LearningPlatform.Application.Features.Lectures.Commands.DeleteLecture;
using LearningPlatform.Application.Features.Lectures.Commands.UpdateLecture;
using LearningPlatform.Application.Features.Lectures.Queries.GetAllLectures;
using LearningPlatform.Application.Features.Lectures.Queries.GetLectureById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

/// <summary>
/// Video Lectures &amp; Digital Content | إدارة المحاضرات والفيديوهات التعليمية
/// </summary>
/// <remarks>
/// Allows viewing lecture details, video streams/URLs, and administrative management of course lectures.
/// تفاصيل المحاضرات، روابط الفيديو، وترتيب المحاضرات داخل المنهج.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LecturesController(IMediator mediator) : AppControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Get lecture by ID | جلب تفاصيل محاضرة بالمعرف
    /// </summary>
    /// <remarks>
    /// Returns video URL, title, description, duration, and subject details.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint عند فتح مشغل الفيديو (Video Player Screen) لجلب رابط الفيديو وتفاصيل المحاضرة.
    /// </remarks>
    /// <param name="id">Lecture GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns lecture data and video link.</response>
    /// <response code="404">Lecture not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLectureByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all lectures | جلب جميع المحاضرات
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of all lectures.</response>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllLecturesQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Create new lecture | إضافة محاضرة جديدة
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">Lecture details (Title, SubjectId, VideoUrl, OrderIndex, Description).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Lecture created successfully.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateLectureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Update lecture | تعديل بيانات محاضرة
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Lecture GUID in URL.</param>
    /// <param name="command">Updated lecture details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Lecture updated successfully.</response>
    /// <response code="400">Route ID does not match command ID.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLectureCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest("Route ID and request ID do not match.");
        }

        var result = await _mediator.Send(
            command,
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Delete lecture | حذف محاضرة
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Lecture GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Lecture deleted.</response>
    /// <response code="404">Lecture not found.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteLectureCommand(id),
            cancellationToken);

        return HandleResult(result);
    }
}