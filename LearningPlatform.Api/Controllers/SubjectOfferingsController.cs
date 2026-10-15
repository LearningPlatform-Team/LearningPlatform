using LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.CreateSubjectOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.RemoveTaFromOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferingById;
using LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Semester Course Offerings | المقررات المطروحة فصلياً وتعيين طاقم التدريس
/// </summary>
/// <remarks>
/// Manages courses offered in specific academic terms, assigning Doctors and Teaching Assistants (TAs), and section capacity.
/// إدارة المواد المطروحة في الفصول الدراسية وتعيين الأساتذة والمعيدين لكل شعبة/مقرر.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class SubjectOfferingsController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Create course offering for a term | طرح مقرر في فصل دراسي
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="command">SubjectId, AcademicTermId, DoctorProfileId, and capacity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Offering created successfully.</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectOfferingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update course offering | تعديل بيانات طرح المقرر
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="id">Offering GUID in URL.</param>
    /// <param name="command">Updated offering details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Offering updated.</response>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubjectOfferingCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID must match the command ID.");

        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get offering details by ID | تفاصيل المقرر المطروح
    /// </summary>
    /// <param name="id">Offering GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns offering data, doctor, TAs, and term.</response>
    /// <response code="404">Offering not found.</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA,Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectOfferingByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Query all course offerings | استعلام جميع المقررات المطروحة
    /// </summary>
    /// <param name="query">Filter by TermId, Department, or Doctor.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of course offerings.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,TA,Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetSubjectOfferingsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Assign Teaching Assistant (TA) to course offering | تعيين معيد للمقرر
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="offeringId">Offering GUID.</param>
    /// <param name="request">TA Profile GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">TA assigned successfully.</response>
    [HttpPost("{offeringId:guid}/tas")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignTa(
        Guid offeringId,
        [FromBody] AssignTaCommand request,
        CancellationToken cancellationToken)
    {
        var command = new AssignTaCommand
        {
            OfferingId = offeringId,
            TaProfileId = request.TaProfileId
        };

        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove Teaching Assistant from course offering | إزالة معيد من المقرر
    /// </summary>
    /// <remarks>Role: Admin only.</remarks>
    /// <param name="subjectOfferingId">Offering GUID.</param>
    /// <param name="taProfileId">TA Profile GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">TA removed.</response>
    [HttpDelete("{subjectOfferingId:guid}/tas/{taProfileId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveTa(
        Guid subjectOfferingId,
        Guid taProfileId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new RemoveTaFromOfferingCommand(
                subjectOfferingId,
                taProfileId),
            cancellationToken);

        return HandleResult(result);
    }
}