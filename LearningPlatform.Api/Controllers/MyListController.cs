using LearningPlatform.Application.Features.Mylist.Command.ClearMyList;
using LearningPlatform.Application.Features.Mylist.Command.CreateList;
using LearningPlatform.Application.Features.Mylist.Command.RemoveFromList;
using LearningPlatform.Application.Features.Mylist.Queries.GetMyListQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Student Watchlist &amp; Saved Lectures | قائمتي والمحاضرات المحفوظة
    /// </summary>
    /// <remarks>
    /// Allows students to bookmark lectures, manage a personal watchlist, and watch saved videos later.
    /// إدارة قائمة المحاضرات المحفوظة والمفضلة للمشاهدة لاحقاً للطالب.
    /// </remarks>
    [Route("api/my-list")]
    [Authorize(Roles = "Student")]
    [ApiController]
    [Produces("application/json")]
    public class MyListController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get student's saved watchlist | جلب قائمة المحاضرات المحفوظة
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint في شاشة "قائمتي" / "محفوظاتي" (My List Screen) لعرض الدروس التي حفظها الطالب.
        /// </remarks>
        /// <param name="search">Optional title search filter.</param>
        /// <param name="pageNumber">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns paginated saved lectures.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyList(
            [FromQuery] string? search,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetMyListQuery(search, pageNumber, pageSize), cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Add lecture to watchlist | إضافة محاضرة إلى قائمتي
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استدع هذا الـ Endpoint عند الضغط على أيقونة الإشارة المرجعية / "حفظ" بجانب أي محاضرة.
        /// </remarks>
        /// <param name="command">Lecture ID to bookmark.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Lecture added to watchlist.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] AddToMyListCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Remove lecture from watchlist | إزالة محاضرة من قائمتي
        /// </summary>
        /// <param name="lectureId">Lecture GUID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Lecture removed from watchlist.</response>
        [HttpDelete("{lectureId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Remove(Guid lectureId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveFromMyListCommand(lectureId), cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Clear all saved lectures in watchlist | مسح جميع المحاضرات من القائمة
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Watchlist cleared.</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Clear(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ClearMyListCommand(), cancellationToken);
            return HandleResult(result);
        }
    }
}
