using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Books.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetBookByIdQuery, ResultT<BookResponse>>
{
    public async Task<ResultT<BookResponse>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        var book = await context.Books
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new BookResponse(
                x.Id,
                x.SubjectId,
                x.Title,
                x.Description,
                x.FileStorageKey,
                x.UploadedByUserId))
            .FirstOrDefaultAsync(cancellationToken);

        if (book is null)
        {
            return ResultT<BookResponse>.Failure(
                "Book was not found.",
                ErrorType.NotFound);
        }

        return ResultT<BookResponse>.Success(book);
    }
}