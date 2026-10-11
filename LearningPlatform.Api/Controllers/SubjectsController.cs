using LearningPlatform.Application.Features.Subjects.Commands.CreateSubject;
using LearningPlatform.Application.Features.Subjects.Commands.DeleteSubject;
using LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
public class SubjectsController : AppControllerBase
{
    private readonly IMediator _mediator;

    public SubjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetSubjectByIdQuery(id),
            cancellationToken);
        return HandleResult(result);
    }
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "The route ID does not match the subject ID in the request body."
            });
        }

        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSubjectCommand(id);

        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }
}