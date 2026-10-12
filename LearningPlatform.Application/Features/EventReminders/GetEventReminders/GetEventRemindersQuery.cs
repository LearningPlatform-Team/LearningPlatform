using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
namespace LearningPlatform.Application.Features.EventReminders.GetEventReminders
{
    public sealed record GetEventRemindersQuery(
        Guid UserId,
        bool UpcomingOnly = true,
        int PageNumber = 1,
        int PageSize = 20
    ) : IRequest<ResultT<PaginatedList<EventReminderDto>>>;
}
