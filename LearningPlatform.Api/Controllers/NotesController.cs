using LearningPlatform.Application.Features.Note.Command.CreateNote;
using LearningPlatform.Application.Features.Note.Command.DeleteNote;
using LearningPlatform.Application.Features.Note.Command.UpdateNote;
using LearningPlatform.Application.Features.Note.Queries.GetAllNotesQuery;
using LearningPlatform.Application.Features.Note.Queries.GetNoteByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Student Personal Study Notes | الملاحظات الدراسية الشخصية للطالب
/// </summary>
/// <remarks>
/// Allows students to create, organize, color-code, and manage study notes during lectures.
/// تدوين الملاحظات الدراسية، تمييزها بالألوان، وحفظها للرجوع إليها أثناء المذاكرة.
/// </remarks>
[Authorize(Roles = "Student")]
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class NotesController : AppControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create new study note | إنشاء ملاحظة جديدة
    /// </summary>
    /// <remarks>
    /// **Role: Student only.**
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - يدعم تمرير لون للملاحظة (Hex code مثل `#FFD700` أو اسم لون) لتلوين البطاقة في التطبيق.
    /// </remarks>
    /// <param name="command">Title, Content, and optional Color hex.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Note created successfully.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateNoteCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get note by ID | جلب تفاصيل ملاحظة بالمعرف
    /// </summary>
    /// <param name="id">Note GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns note data.</response>
    /// <response code="404">Note not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all student's study notes | جلب جميع ملاحظات الطالب
    /// </summary>
    /// <remarks>
    /// Supports keyword search across note title and body content.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint في شاشة "ملاحظاتي" (Notes Screen) مع البحث والتصفح الشبكي (Grid/List View).
    /// </remarks>
    /// <param name="search">Search keyword in note title/content.</param>
    /// <param name="pageNumber">Page index.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns paginated list of notes.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllNotesQuery(search, pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete note | حذف ملاحظة
    /// </summary>
    /// <param name="id">Note GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Note deleted.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteNoteCommand(id), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Partially update a note | تعديل الملاحظة (العنوان، المحتوى، أو اللون)
    /// </summary>
    /// <param name="id">Note GUID.</param>
    /// <param name="request">Fields to update (Title, Content, Color).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Note updated successfully.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Patch(Guid id, [FromBody] PatchNoteRequest request, CancellationToken cancellationToken)
    {
        var command = new PatchNoteCommand(id, request.Title, request.Content, request.Color);
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}