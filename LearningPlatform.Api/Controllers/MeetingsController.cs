using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Application.Features.Meetings.CreateMeeting;
using LearningPlatform.Application.Features.Meetings.EndMeeting;
using LearningPlatform.Application.Features.Meetings.GetMeetingDetails;
using LearningPlatform.Application.Features.Meetings.JoinMeeting;
using LearningPlatform.Application.Features.Meetings.StartMeeting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    /// <summary>
    /// Live Virtual Classes &amp; Meetings | الفصول الافتراضية والاجتماعات المباشرة
    /// </summary>
    /// <remarks>
    /// Integrates real-time virtual lectures, live presence tracking, and meeting lifecycle (Create, Start, Join, End).
    /// إدارة المحاضرات التفاعلية الحية ومتابعة الحاضرين في الوقت الفعلي.
    /// </remarks>
    [Authorize]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MeetingsController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MeetingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create new virtual meeting/lecture | إنشاء اجتماع أو محاضرة أونلاين
        /// </summary>
        /// <param name="command">Meeting title, offeringId, scheduled start time, and settings.</param>
        /// <response code="200">Meeting created successfully.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] CreateMeetingCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Start meeting session | بدء جلسة البث المباشر
        /// </summary>
        /// <param name="id">Meeting GUID.</param>
        /// <response code="200">Meeting marked as Active/In-Progress.</response>
        [HttpPost("{id:guid}/start")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Start(Guid id)
        {
            var result = await _mediator.Send(new StartMeetingCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// End meeting session | إنهاء جلسة المحاضرة المباشرة
        /// </summary>
        /// <param name="id">Meeting GUID.</param>
        /// <response code="200">Meeting ended.</response>
        [HttpPost("{id:guid}/end")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> End(Guid id)
        {
            var result = await _mediator.Send(new EndMeetingCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Join a meeting | الانضمام إلى محاضرة حية
        /// </summary>
        /// <remarks>
        /// Allows a student or staff member to join an active meeting room and registers their presence.
        ///
        /// **ملاحظات لمطور الـ UI:**
        /// - استدع هذا الـ Endpoint عند ضغط الطالب على "Join Class" / "انضمام للمحاضرة".
        /// </remarks>
        /// <param name="command">Meeting join token/code.</param>
        /// <response code="200">Returns join credentials or connection token.</response>
        [HttpPost("join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Join([FromBody] JoinMeetingCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Get meeting details | تفاصيل الاجتماع
        /// </summary>
        /// <param name="id">Meeting GUID.</param>
        /// <response code="200">Returns meeting details, status, and duration.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetMeetingDetailsQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Get live active participants | قائمة المتواجدين حالياً في الاجتماع
        /// </summary>
        /// <remarks>Returns real-time list of online users in this meeting room.</remarks>
        /// <param name="id">Meeting GUID.</param>
        /// <param name="presenceTracker">Injected presence tracker service.</param>
        /// <response code="200">Returns list of current live participant user IDs.</response>
        [HttpGet("{id:guid}/participants")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLiveParticipants(
            Guid id,
            [FromServices] IMeetingPresenceTracker presenceTracker)
        {
            var participants = await presenceTracker.GetParticipantsAsync(id.ToString());
            return Ok(participants);
        }
    }
}
