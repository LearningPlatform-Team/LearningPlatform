using LearningPlatform.Application.Features.Enrollments.Commands.CreateEnrollment;
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


}
