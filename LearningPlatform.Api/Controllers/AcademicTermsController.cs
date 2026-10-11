using LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
public class AcademicTermsController(IMediator mediator) : AppControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateAcademicTermCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }
}