using LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Commands.DeleteAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Commands.UpdateAcademicTerm;
using LearningPlatform.Application.Features.AcademicTerms.Queries.GetAcademicTermById;
using LearningPlatform.Application.Features.AcademicTerms.Queries.GetAllAcademicTerms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicTermsController(IMediator mediator) : AppControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAllAcademicTermsQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAcademicTermByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAcademicTermCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
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

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
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