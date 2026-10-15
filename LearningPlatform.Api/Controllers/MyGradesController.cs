using LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Student Grades &amp; Academic Transcript | درجات الطالب والسجل التراكمي
    /// </summary>
    /// <remarks>
    /// Allows students to view their exam scores, course grades, GPA, and assessment breakdowns.
    /// استعلام الطالب عن درجاته في المواد، نتائج الامتحانات والتقييمات، ومعدله الفصلي.
    /// </remarks>
    [Route("api/my-grades")]
    [Authorize(Roles = "Student")]
    [ApiController]
    [Produces("application/json")]
    public class MyGradesController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MyGradesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get current student's grades | جلب نتائج ودرجات الطالب
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        /// Optionally filter by an academic term (semester). If `termId` is omitted, returns grades for the current active semester.
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint في شاشة "درجاتي" / "نتائج الامتحانات" في التطبيق.
        /// </remarks>
        /// <param name="termId">Optional AcademicTermId to view historical grades for a specific term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns grades for each course, assignments, midterms, and final scores.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden — caller is not a Student.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyGrades([FromQuery] Guid? termId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyGradesQuery(termId), cancellationToken);
            return HandleResult(result);
        }
    }
}
