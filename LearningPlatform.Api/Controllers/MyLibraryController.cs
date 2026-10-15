using LearningPlatform.Application.Features.MyLibrary.Quiers.GetBookById;
using LearningPlatform.Application.Features.MyLibrary.Quiers.GetMyLibrary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Student Personal Digital Library | مكتبة الطالب الرقمية
    /// </summary>
    /// <remarks>
    /// Search, browse, and read digital textbooks and course books assigned to the student's enrolled courses.
    /// استعراض الكتب والمراجع المتاحة للطالب بناءً على المواد المسجل بها والبحث في المكتبة.
    /// </remarks>
    [Route("api/my-library")]
    [Authorize(Roles = "Student")]
    [ApiController]
    [Produces("application/json")]
    public class MyLibraryController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyLibraryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Browse student's accessible library books | تصفح والبحث في مكتبة الطالب
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        /// Search by book title/author or filter by a specific subject.
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint في شاشة "المكتبة" (Library Screen) مع شريط البحث والفلترة حسب المادة.
        /// </remarks>
        /// <param name="subjectId">Optional subject filter.</param>
        /// <param name="search">Optional search keyword (title/author).</param>
        /// <param name="pageNumber">Page index (default 1).</param>
        /// <param name="pageSize">Page size (default 20).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns paginated list of books.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

        /// <summary>
        /// Get book reader details &amp; PDF download URL | فتح كتاب وقراءة التفاصيل ورابط التحميل
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        /// Returns book metadata, cover picture, and PDF link for reading.
        /// </remarks>
        /// <param name="id">Book GUID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns book details and PDF file link.</response>
        /// <response code="404">Book not found.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBookByIdQuery(id), cancellationToken);
            return HandleResult(result);
        }
    }
}
