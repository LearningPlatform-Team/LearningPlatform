using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Meetings.Common;
using LearningPlatform.Application.Features.Meetings.CreateMeeting;
using LearningPlatform.Application.Features.Meetings.EndMeeting;
using LearningPlatform.Application.Features.Meetings.GetMeetingDetails;
using LearningPlatform.Application.Features.Meetings.JoinMeeting;
using LearningPlatform.Application.Features.Meetings.StartMeeting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class MeetingsController : AppControllerBase
    {
        private readonly IMediator _mediator;

        public MeetingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new instant or scheduled meeting with Google Meet integration.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeetingCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Starts the meeting lifecycle (Only Host or Admin).
        /// </summary>
        [HttpPost("{id:guid}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            var result = await _mediator.Send(new StartMeetingCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Ends an active meeting (Only Host or Admin).
        /// </summary>
        [HttpPost("{id:guid}/end")]
        public async Task<IActionResult> End(Guid id)
        {
            var result = await _mediator.Send(new EndMeetingCommand(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Joins a meeting. Validates participant join code and automatically marks student attendance.
        /// </summary>
        [HttpPost("join")]
        public async Task<IActionResult> Join([FromBody] JoinMeetingCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Gets detailed meeting information by meeting ID.
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetMeetingDetailsQuery(id));
            return HandleResult(result);
        }

        /// <summary>
        /// Returns the live participant roster and real-time media states (mic, cam, screen sharing).
        /// </summary>
        [HttpGet("{id:guid}/participants")]
        public async Task<IActionResult> GetLiveParticipants(
            Guid id,
            [FromServices] IMeetingPresenceTracker presenceTracker)
        {
            var participants = await presenceTracker.GetParticipantsAsync(id.ToString());
            return Ok(participants);
        }
    }
}
