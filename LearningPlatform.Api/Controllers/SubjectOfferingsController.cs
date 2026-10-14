using LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.CreateSubjectOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.RemoveTaFromOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;
using LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferingById;
using LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubjectOfferingsController(IMediator mediator) : AppControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectOfferingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
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

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA,Student")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectOfferingByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor,TA,Student")]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetSubjectOfferingsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{offeringId:guid}/tas")]
    [Authorize(Roles = "Admin")]
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

    [HttpDelete("{subjectOfferingId:guid}/tas/{taProfileId:guid}")]
    [Authorize(Roles = "Admin")]
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