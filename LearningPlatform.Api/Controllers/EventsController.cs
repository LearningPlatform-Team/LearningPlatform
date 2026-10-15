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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.Api.Controllers;

/// <summary>
/// Academic Calendar, Deadlines &amp; Event Reminders | التقويم الأكاديمي، المواعيد الهامة والتذكيرات
/// </summary>
/// <remarks>
/// Manages university events (Exams, Assignments, Seminars, Holidays) and personal push-notification reminders for students.
/// إدارة الفعاليات الجامعية ومواعيد تسليم التكليفات والامتحانات وتحديد تنبيهات مسبقة للطالب.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class EventsController : AppControllerBase
{
    private readonly ISender _sender;

    public EventsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create new event | إنشاء فعالية أو موعد تسليم/امتحان جديد
    /// </summary>
    /// <remarks>Roles: Admin, Doctor, TA.</remarks>
    /// <param name="request">Event info (Title, Description, Type, StartsAtUtc, EndsAtUtc, Location, SubjectOfferingId).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Event created successfully.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,TA")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Update existing event | تعديل فعالية
    /// </summary>
    /// <remarks>Roles: Admin, Doctor, TA.</remarks>
    /// <param name="eventId">Event GUID.</param>
    /// <param name="request">Updated event info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Event updated successfully.</response>
    [HttpPut("{eventId:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateEvent(
        Guid eventId,
        [FromBody] UpdateEventRequestDto request,
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

        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete event | حذف فعالية
    /// </summary>
    /// <remarks>Roles: Admin, Doctor, TA.</remarks>
    /// <param name="eventId">Event GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Event deleted successfully.</response>
    [HttpDelete("{eventId:guid}")]
    [Authorize(Roles = "Admin,Doctor,TA")]
    [ProducesResponseType(StatusCodes.Status200OK)]
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

    /// <summary>
    /// Get event by ID | جلب تفاصيل فعالية بالمعرف
    /// </summary>
    /// <param name="eventId">Event GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns event details.</response>
    /// <response code="404">Event not found.</response>
    [HttpGet("{eventId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEventById(Guid eventId, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var query = new GetEventByIdQuery(
            eventId,
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA());
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Query and filter events | استعلام وفلترة الفعاليات في التقويم
    /// </summary>
    /// <remarks>
    /// Filter by EventType (Lecture, Exam, Assignment, Seminar, Holiday), SubjectOfferingId, Date range, or upcoming only.
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint في شاشة التقويم (Calendar View) أو شاشة قائمة الفعاليات.
    /// </remarks>
    /// <param name="type">Optional event type filter.</param>
    /// <param name="subjectOfferingId">Optional course offering filter.</param>
    /// <param name="fromUtc">Start date filter (UTC).</param>
    /// <param name="toUtc">End date filter (UTC).</param>
    /// <param name="upcomingOnly">Filter to future events only.</param>
    /// <param name="pageNumber">Page index (default 1).</param>
    /// <param name="pageSize">Page size (default 20).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns paginated events list.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvents(
        [FromQuery] EventType? type = null,
        [FromQuery] Guid? subjectOfferingId = null,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] bool upcomingOnly = false,
        [FromQuery] int pageNumber = 1,
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
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get upcoming events | جلب أقرب الفعاليات القادمة
    /// </summary>
    /// <remarks>
    /// Returns the nearest upcoming events for quick display on home banners or widgets.
    /// </remarks>
    /// <param name="take">Number of events to retrieve (default 5).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of nearest upcoming events.</response>
    [HttpGet("upcoming")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUpcomingEvents([FromQuery] int take = 5, CancellationToken cancellationToken = default)
    {
        var currentUserId = User.GetUserId();
        var query = new GetUpcomingEventsQuery(
            currentUserId,
            User.IsAdmin(),
            User.IsDoctor(),
            User.IsTA(),
            take);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Set a reminder for an event | تفعيل تذكير بفعالية
    /// </summary>
    /// <remarks>
    /// Sets a notification reminder for the current user (e.g. 15, 30, or 60 minutes before the event starts).
    ///
    /// **ملاحظات لمطور الـ UI:**
    /// - استخدم هذا الـ Endpoint عند الضغط على أيقونة الجرس / التذكير بجانب الفعالية.
    /// </remarks>
    /// <param name="eventId">Event GUID.</param>
    /// <param name="request">Minutes before start to trigger reminder.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Reminder created/updated successfully.</response>
    [HttpPut("{eventId:guid}/reminder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SetEventReminder(
        Guid eventId,
        [FromBody] SetEventReminderRequestDto request,
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
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove event reminder | إلغاء التذكير بفعالية
    /// </summary>
    /// <param name="eventId">Event GUID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Reminder removed.</response>
    [HttpDelete("{eventId:guid}/reminder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteEventReminder(Guid eventId, CancellationToken cancellationToken)
    {
        var currentUserId = User.GetUserId();
        var command = new DeleteEventReminderCommand(eventId, currentUserId);
        var result = await _sender.Send(command, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get my active reminders | جلب قائمة تذكيراتي النشطة
    /// </summary>
    /// <param name="upcomingOnly">Filter to upcoming reminders only.</param>
    /// <param name="pageNumber">Page index.</param>
    /// <param name="pageSize">Page size.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Returns list of event reminders set by the current user.</response>
    [HttpGet("reminders")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEventReminders(
        [FromQuery] bool upcomingOnly = true,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = User.GetUserId();
        var query = new GetEventRemindersQuery(
            currentUserId,
            upcomingOnly,
            pageNumber,
            pageSize);
        var result = await _sender.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
