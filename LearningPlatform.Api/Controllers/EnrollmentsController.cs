using LearningPlatform.Application.Features.Enrollments.Commands.CancelEnrollment;
using LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;
using LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EnrollmentsController(IMediator mediator) : AppControllerBase
{
    /// <summary>
    /// Enrolls a student in a subject offering.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateEnrollment(
    [FromBody] CreateEnrollmentCommand command,
    CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{enrollmentId:guid}")]
    public async Task<IActionResult> CancelEnrollment(
        Guid enrollmentId,
        CancellationToken cancellationToken)
    {
        var command = new CancelEnrollmentCommand(enrollmentId);
        var result = await mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetMyEnrollmentHistoryQuery(),
            cancellationToken);

        return HandleResult(result);
    }
}
