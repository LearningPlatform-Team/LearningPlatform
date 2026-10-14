using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Common.Models;
using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Books.Queries.GetBooks;

public sealed class GetBooksQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetBooksQuery, ResultT<PagedResult<BookResponse>>>
{
    public async Task<ResultT<PagedResult<BookResponse>>> Handle(
        GetBooksQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Books.AsNoTracking();

        if (request.SubjectId.HasValue)
        {
            query = query.Where(b => b.SubjectId == request.SubjectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(b => b.Title.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .OrderBy(b => b.Title)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => new BookResponse(
                b.Id,
                b.SubjectId,
                b.Title,
                b.Description,
                b.FileStorageKey,
                b.UploadedByUserId))
            .ToListAsync(cancellationToken);

        var response = new PagedResult<BookResponse>(
            books,
            request.PageNumber,
            request.PageSize,
            totalCount);

        return ResultT<PagedResult<BookResponse>>.Success(response);
    }
}