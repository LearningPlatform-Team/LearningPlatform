using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Application.Features.Mylist.Queries.Shared;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Mylist.Queries.GetMyListQuery
{
    public class GetMyListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
         : IRequestHandler<GetMyListQuery, ResultT<PaginatedList<MyListItemResponse>>>
    {
        public async Task<ResultT<PaginatedList<MyListItemResponse>>> Handle(GetMyListQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<PaginatedList<MyListItemResponse>>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var enrolledOfferingIds = context.Enrollments
                .AsNoTracking()
                .Where(en => en.StudentId == userId && en.Status == EnrollmentStatus.Enrolled)
                .Select(en => en.OfferingId);

            var query = context.MyListItems
                .AsNoTracking()
                .Where(m => m.UserId == userId
                         && m.Lecture.IsPublished
                         && enrolledOfferingIds.Contains(m.Lecture.SubjectOfferingId));

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(m =>
                    m.Lecture.Title.ToLower().Contains(search) ||
                    m.Lecture.Offering.Subject.Name.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(m => m.Lecture.Offering.Subject.Name)
                .ThenBy(m => m.Lecture.SortOrder)
                .ThenBy(m => m.LectureId)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(m => new MyListItemResponse(
                    m.LectureId,
                    m.Lecture.Title,
                    m.Lecture.Description,
                    m.Lecture.DurationMinutes,
                    m.Lecture.SubjectOfferingId,
                    m.Lecture.Offering.Subject.Name))
                .ToListAsync(cancellationToken);

            var page = new PaginatedList<MyListItemResponse>(items, totalCount, request.PageNumber, request.PageSize);

            return ResultT<PaginatedList<MyListItemResponse>>.Success(page);
        }
    }
}

