using LearningPlatform.Application.Features.Mylist.Command.ClearMyList;
using LearningPlatform.Application.Features.Mylist.Command.CreateList;
using LearningPlatform.Application.Features.Mylist.Command.RemoveFromList;
using LearningPlatform.Application.Features.Mylist.Queries.GetMyListQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Route("api/my-list")]
    [Authorize(Roles = "Student")]
    public class MyListController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyList([FromQuery] string? search,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 20,CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMyListQuery(search, pageNumber, pageSize), cancellationToken);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddToMyListCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete("{lectureId:guid}")]
        public async Task<IActionResult> Remove(Guid lectureId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveFromMyListCommand(lectureId), cancellationToken);
            return HandleResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Clear(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ClearMyListCommand(), cancellationToken);
            return HandleResult(result);
        }
    }
}
