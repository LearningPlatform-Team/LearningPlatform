using LearningPlatform.Api.Extensions;
using LearningPlatform.Application.Features.Home.GetAdminHome;
using LearningPlatform.Application.Features.Home.GetStudentHome;
using LearningPlatform.Application.Features.Home.GetTeachingStaffHome;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Dashboard &amp; Home Screen Data | بيانات الشاشة الرئيسية ولوحة التحكم
    /// </summary>
    /// <remarks>
    /// Provides tailored home screen feeds for Students, Teaching Staff (Doctors &amp; TAs), and Admins.
    /// يوفر بيانات الشاشة الرئيسية المخصصة للطلاب، أعضاء هيئة التدريس (الدكاترة والمعيدين)، والمديرين.
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class HomeController : AppControllerBase
    {
        private readonly ISender _sender;

        public HomeController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Get Student Home Dashboard | جلب بيانات الشاشة الرئيسية للطالب
        /// </summary>
        /// <remarks>
        /// **Role: Student only.**
        /// Returns the student's enrolled subjects, upcoming lectures/sessions, recent events, and announcements.
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استخدم هذا الـ Endpoint لملء الشاشة الرئيسية لتطبيق الطالب (المواد الحالية، المحاضرات القادمة، الإشعارات السريعة).
        /// </remarks>
        /// <param name="itemsCount">Number of recent items to retrieve (default is 5).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns student dashboard data.</response>
        /// <response code="401">Unauthorized — requires Bearer token.</response>
        /// <response code="403">Forbidden — caller is not a Student.</response>
        [HttpGet("student")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetStudentHome([FromQuery] int itemsCount = 5, CancellationToken cancellationToken = default)
        {
            var studentId = User.GetUserId();
            var query = new GetStudentHomeQuery(studentId, itemsCount);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get Teaching Staff Dashboard | جلب بيانات لوحة تحكم أعضاء هيئة التدريس (دكتور/معيد)
        /// </summary>
        /// <remarks>
        /// **Roles: Doctor, TA.**
        /// Returns teaching schedule, assigned subject offerings, pending attendance, and upcoming sessions.
        /// </remarks>
        /// <param name="itemsCount">Number of recent items to retrieve (default is 5).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns teaching staff dashboard data.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden — caller is not Doctor or TA.</response>
        [HttpGet("teaching-staff")]
        [Authorize(Roles = "Doctor,TA")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetTeachingStaffHome([FromQuery] int itemsCount = 5, CancellationToken cancellationToken = default)
        {
            var currentUserId = User.GetUserId();
            var query = new GetTeachingStaffHomeQuery(
                currentUserId,
                User.IsDoctor(),
                User.IsTA(),
                itemsCount);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }

        /// <summary>
        /// Get Admin Dashboard | جلب إحصائيات وبيانات لوحة تحكم الإدارة
        /// </summary>
        /// <remarks>
        /// **Role: Admin only.**
        /// Returns platform metrics (total students, doctors, active terms, recent audit logs).
        /// </remarks>
        /// <param name="itemsCount">Number of items to retrieve (default is 5).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns admin dashboard metrics.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden — caller is not an Admin.</response>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAdminHome([FromQuery] int itemsCount = 5, CancellationToken cancellationToken = default)
        {
            var currentUserId = User.GetUserId();
            var query = new GetAdminHomeQuery(currentUserId, User.IsAdmin(), itemsCount);
            var result = await _sender.Send(query, cancellationToken);
            return HandleResult(result);
        }
    }
}
