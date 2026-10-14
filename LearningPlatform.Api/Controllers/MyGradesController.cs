using LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{

    [Route("api/my-grades")]
    [Authorize(Roles = "Student")]
    public class MyGradesController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyGradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyGrades([FromQuery] Guid? termId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyGradesQuery(termId), cancellationToken);
            return HandleResult(result);
        }


    }
}
