using LearningPlatform.Application.Features.Subjects.Commands.CreateSubject;
using LearningPlatform.Application.Features.Subjects.Commands.DeleteSubject;
using LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;
using LearningPlatform.Application.Features.Subjects.Queries.GetMySubjects;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectBooks;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;
using LearningPlatform.Application.Features.Subjects.Queries.GetSubjectLectures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectsController(IMediator mediator) : AppControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectByIdQuery(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMySubjects(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMySubjectsQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSubjectCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route ID does not match command ID.");

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
            new DeleteSubjectCommand(id),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{subjectId:guid}/lectures")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetSubjectLectures(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectLecturesQuery(subjectId),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("{subjectId:guid}/books")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetSubjectBooks(
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetSubjectBooksQuery(subjectId),
            cancellationToken);

        return HandleResult(result);
    }
}