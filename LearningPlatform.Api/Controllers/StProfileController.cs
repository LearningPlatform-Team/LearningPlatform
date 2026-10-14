using LearningPlatform.Application.Features.StProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Route("api/profile")]
    [Authorize (Roles = "Student")]
    public class StProfileController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public StProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentProfileQuery(), cancellationToken);
            return HandleResult(result);
        }
    }
}
