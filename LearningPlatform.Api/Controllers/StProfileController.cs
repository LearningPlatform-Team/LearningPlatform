using LearningPlatform.Application.Features.StProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Student Profile &amp; Academic Data | الملف الشخصي والأكاديمي للطالب
    /// </summary>
    /// <remarks>
    /// Fetches the authenticated student's full profile including Student Code, GPA, department, and account information.
    /// جلب بيانات الملف الشخصي للطالب، الرقم الجامعي، والمعدل التراكمي.
    /// </remarks>
    [Route("api/profile")]
    [Authorize(Roles = "Student")]
    [ApiController]
    [Produces("application/json")]
    public class StProfileController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public StProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get authenticated student profile | جلب بيانات الملف الشخصي للطالب الحالي
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint في شاشة "الملف الشخصي" (Profile Screen) لعرض الاسم، الإيميل، الرقم الأكاديمي، ورقم الهاتف.
        /// </remarks>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns student profile data.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetStudentProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetStudentProfileQuery(), cancellationToken);
            return HandleResult(result);
        }
    }
}
