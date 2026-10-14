using LearningPlatform.Application.Features.Enrollments.Commands.CancelEnrollment;
using LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;
using LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;
using LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController(IMediator mediator) : AppControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateEnrollment(
        [FromBody] CreateEnrollmentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
    [FromQuery] GetEnrollmentsQuery query,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{enrollmentId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CancelEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var command = new CancelEnrollmentCommand(enrollmentId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("my-history")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyHistory(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMyEnrollmentHistoryQuery(),
            cancellationToken);

        return HandleResult(result);
    }
}