using LearningPlatform.Application.Features.Note.Command.CreateNote;
using LearningPlatform.Application.Features.Note.Command.DeleteNote;
using LearningPlatform.Application.Features.Note.Command.UpdateNote;
using LearningPlatform.Application.Features.Note.Queries.GetAllNotesQuery;
using LearningPlatform.Application.Features.Note.Queries.GetNoteByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;
[Authorize(Roles = "Student")]
[Route("api/[controller]")]

public class NotesController : AppControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNoteCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id),cancellationToken);
        return HandleResult(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 20,CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllNotesQuery(search, pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteNoteCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Patch(Guid id, [FromBody] PatchNoteRequest request, CancellationToken cancellationToken)
    {
        var command = new PatchNoteCommand(id, request.Title, request.Content, request.Color);
        var result = await _mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

}