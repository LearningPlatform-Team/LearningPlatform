using LearningPlatform.Api.Extensions;
using LearningPlatform.Application.Features.EventReminders.CreateEventReminder;
using LearningPlatform.Application.Features.EventReminders.DeleteEventReminder;
using LearningPlatform.Application.Features.EventReminders.GetEventReminders;
using LearningPlatform.Application.Features.Events.CreateEvent;
using LearningPlatform.Application.Features.Events.DeleteEvent;
using LearningPlatform.Application.Features.Events.GetEventById;
using LearningPlatform.Application.Features.Events.GetEvents;
using LearningPlatform.Application.Features.Events.GetUpcomingEvents;
using LearningPlatform.Application.Features.Events.UpdateEvent;
using LearningPlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EventsController : AppControllerBase
{
    private readonly ISender _sender;

    public EventsController(ISender sender)
    {
        _sender = sender;
    }
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,TA")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDto request, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new CreateEventCommand(
            request.Title,
            request.Description,
            request.Type,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Location,
            request.SubjectOfferingId,
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());
        var result = await _sender.Send(command,cancellationToken);
        return HandleResult(result);
    }


    [HttpPut("{eventId:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA")]
    public async Task<IActionResult> UpdateEvent(Guid eventId,[FromBody] UpdateEventRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new UpdateEventCommand(
            eventId,
            request.Title,
            request.Description,
            request.Type,
            request.StartsAtUtc,
            request.EndsAtUtc,
            request.Location,
            request.SubjectOfferingId,
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());

        var result = await _sender.Send(command,cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{eventId:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA")]
    public async Task<IActionResult> DeleteEvent(Guid eventId, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new DeleteEventCommand(
            eventId,
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }


    [HttpGet("{eventId:guid}")]
    public async Task<IActionResult> GetEventById(Guid eventId,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var query = new GetEventByIdQuery(
            eventId,
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());
        var result = await _sender.Send(query,cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] EventType? type = null, [FromQuery] Guid? subjectOfferingId = null,
        [FromQuery] DateTime? fromUtc = null, [FromQuery] DateTime? toUtc = null,
        [FromQuery] bool upcomingOnly = false,[FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = User.GetUserId();
        var query = new GetEventsQuery(
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA(),
            type,
            subjectOfferingId,
            fromUtc,
            toUtc,
            upcomingOnly,
            pageNumber,
            pageSize);
        var result = await _sender.Send(query,cancellationToken);
        return HandleResult(result);
    }
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingEvents([FromQuery] int take = 5, CancellationToken cancellationToken = default)
    {
        var currentUserId = User.GetUserId();
        var query = new GetUpcomingEventsQuery(
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA(),
            take);
        var result = await _sender.Send(query,cancellationToken);
        return HandleResult(result);
    }
    [HttpPut("{eventId:guid}/reminder")]
    public async Task<IActionResult> SetEventReminder(Guid eventId,[FromBody] SetEventReminderRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new SetEventReminderCommand(
            eventId,
            currentUserId,
            request.MinutesBefore,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());
        var result = await _sender.Send(command,cancellationToken);
        return HandleResult(result);
    }
    [HttpDelete("{eventId:guid}/reminder")]
    public async Task<IActionResult> DeleteEventReminder(Guid eventId,CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new DeleteEventReminderCommand(eventId,currentUserId);
        var result = await _sender.Send(command,cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("reminders")]
    public async Task<IActionResult> GetEventReminders([FromQuery] bool upcomingOnly = true,[FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,CancellationToken cancellationToken = default)
    {
        var currentUserId = User.GetUserId();
        var query = new GetEventRemindersQuery(
            currentUserId,
            upcomingOnly,
            pageNumber,
            pageSize);
        var result = await _sender.Send(query,cancellationToken);
        return HandleResult(result);
    }
}

