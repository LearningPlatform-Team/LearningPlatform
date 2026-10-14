using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Application.Features.MyLibrary.Shared;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.MyLibrary.Quiers.GetMyLibrary
{
    public class GetMyLibraryQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
          : IRequestHandler<GetMyLibraryQuery, ResultT<PaginatedList<BookResponse>>>
    {
        public async Task<ResultT<PaginatedList<BookResponse>>> Handle(GetMyLibraryQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<PaginatedList<BookResponse>>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var enrolledSubjectIds = context.Enrollments
                .AsNoTracking()
                .Where(en => en.StudentId == userId && en.Status == EnrollmentStatus.Enrolled)
                .Select(en => en.SubjectOffering.SubjectId);

            var query = context.Books
                .AsNoTracking()
                .Where(b => b.SubjectId == null || enrolledSubjectIds.Contains(b.SubjectId.Value));

            if (request.SubjectId.HasValue)
                query = query.Where(b => b.SubjectId == request.SubjectId.Value);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(b => b.Title)
                .ThenBy(b => b.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BookResponse(
                    b.Id,
                    b.Title,
                    b.Description,
                    b.SubjectId,
                    b.Subject != null ? b.Subject.Name : null))
                .ToListAsync(cancellationToken);

            var page = new PaginatedList<BookResponse>(items, totalCount, request.PageNumber, request.PageSize);

            return ResultT<PaginatedList<BookResponse>>.Success(page);
        }
    }
}
