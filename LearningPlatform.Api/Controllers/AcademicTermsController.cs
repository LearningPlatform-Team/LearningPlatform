using LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Commands.DeleteAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Commands.UpdateAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Queries.GetAcademicTermById;
using LearningPlatform.Application.Features.AcademicTerms.Queries.GetAllAcademicTerms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Academic Semesters &amp; Terms | الفصول والسنوات الدراسية
/// </summary>
/// <remarks>
/// Configuration of academic semesters (e.g. Fall 2026, Spring 2027), active term flags, and semester date boundaries.
/// إدارة الفصول الدراسية وتحديد الفصل الحالي النشط ومواعيد بداية ونهاية كل ترم.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class AcademicTermsController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Get all academic terms | جلب قائمة جميع الفصول الدراسية
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of academic terms.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAllAcademicTermsQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get academic term by ID | جلب تفاصيل فصل دراسي
    /// </summary>
    /// <param name="id">Academic Term GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns academic term details.</response>
    /// <response code="404">Term not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAcademicTermByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Create new academic term | إضافة فصل دراسي جديد
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">Term details (Name, Code, StartDate, EndDate, IsActive).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Term created successfully.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAcademicTermCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update academic term | تعديل فصل دراسي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Term GUID in URL.</param>
    /// <param name="command">Updated term details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Term updated.</response>
    /// <response code="400">Route ID must match command ID.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAcademicTermCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID must match the command ID.");

        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete academic term | حذف فصل دراسي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Term GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Term deleted.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new DeleteAcademicTermCommand(id),
            cancellationToken);

        return HandleResult(result);
    }
}