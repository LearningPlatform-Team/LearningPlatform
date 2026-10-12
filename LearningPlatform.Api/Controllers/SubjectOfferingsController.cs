using LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;
using LearningPlatform.Application.Features.SubjectOfferings.Commands.UpdateSubjectOffering;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectOfferingsController(IMediator mediator) : AppControllerBase
{
    [HttpPut("{id:guid}")]
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

    [Authorize(Roles = "Admin")]
    [HttpPost("{offeringId:guid}/tas")]
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
}

