using LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectOfferingsController(IMediator mediator) : AppControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("{offeringId:guid}/tas")]
    public async Task<IActionResult> AssignTa(
        Guid offeringId,
        [FromBody] AssignTaRequest request,
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

public record AssignTaRequest(Guid TaProfileId);