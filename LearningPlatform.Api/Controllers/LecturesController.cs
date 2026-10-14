using LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;
using LearningPlatform.Application.Features.Lectures.Commands.DeleteLecture;
using LearningPlatform.Application.Features.Lectures.Commands.UpdateLecture;
using LearningPlatform.Application.Features.Lectures.Queries.GetAllLectures;
using LearningPlatform.Application.Features.Lectures.Queries.GetLectureById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LecturesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetLectureByIdQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAllLecturesQuery(),
            cancellationToken);

        return Ok(result);
    }

    //[HttpGet("offering/{subjectOfferingId:guid}")]
    //public async Task<IActionResult> GetByOffering(
    //    Guid subjectOfferingId,
    //    CancellationToken cancellationToken)
    //{
    //    var result = await _mediator.Send(
    //        new GetLecturesByOfferingQuery(subjectOfferingId),
    //        cancellationToken);

    //    return Ok(result);
    //}

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateLectureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateLectureCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteLectureCommand(id),
            cancellationToken);

        return Ok(result);
    }
}