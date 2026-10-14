using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.Events.GetEvents
{

    public sealed record GetEventsQuery(
        Guid RequestedByUserId,
        bool IsAdmin,
        bool IsDoctor,
        bool IsTA,
        EventType? Type = null,
        Guid? SubjectOfferingId = null,
        DateTime? FromUtc = null,
        DateTime? ToUtc = null,
        bool UpcomingOnly = false,
        int PageNumber = 1,
        int PageSize = 20
    ) : IRequest<ResultT<PaginatedList<EventListItemDto>>>;
}
