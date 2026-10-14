using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models.Pagination;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Note.Queries.GetAllNotesQuery
{
    public class GetAllNotesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        : IRequestHandler<GetAllNotesQuery, ResultT<PaginatedList<NoteResponse>>>
    {
        public async Task<ResultT<PaginatedList<NoteResponse>>> Handle(GetAllNotesQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<PaginatedList<NoteResponse>>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var query = context.Notes
                         .AsNoTracking()
                         .Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.Trim().ToLower();
                query = query.Where(n =>
                    n.Title.ToLower().Contains(search) ||
                    (n.Content != null && n.Content.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var notes = await query
                .OrderByDescending(n => n.UpdatedAt)
                .ThenBy(n => n.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(n => new NoteResponse(n.Id, n.Title, n.Content, n.Color, n.UpdatedAt))
                .ToListAsync(cancellationToken);

            var page = new PaginatedList<NoteResponse>(notes, totalCount, request.PageNumber, request.PageSize);

            return ResultT<PaginatedList<NoteResponse>>.Success(page);
        }
    }
}
