using LearningPlatform.Application.Features.MyLibrary.Quiers.GetBookById;
using LearningPlatform.Application.Features.MyLibrary.Quiers.GetMyLibrary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Route("api/my-library")]
    [Authorize(Roles = "Student")]
    public class MyLibraryController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyLibraryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyLibrary(
            [FromQuery] Guid? subjectId,
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMyLibraryQuery(subjectId, search, pageNumber, pageSize), cancellationToken);
            return HandleResult(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBookByIdQuery(id), cancellationToken);
            return HandleResult(result);
        }
    }
}
