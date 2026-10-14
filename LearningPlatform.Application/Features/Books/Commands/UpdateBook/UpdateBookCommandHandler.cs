using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<UpdateBookCommand, Result>
{
    public async Task<Result> Handle(
        UpdateBookCommand request,
        CancellationToken cancellationToken)
    {
        var book = await context.Books
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (book is null)
        {
            return Result.Failure(
                "Book was not found.",
                ErrorType.NotFound);
        }

        if (request.SubjectId.HasValue)
        {
            var subjectExists = await context.Subjects
                .AnyAsync(
                    x => x.Id == request.SubjectId.Value,
                    cancellationToken);

            if (!subjectExists)
            {
                return Result.Failure(
                    "Subject was not found.",
                    ErrorType.NotFound);
            }
        }

        book.SubjectId = request.SubjectId;
        book.Title = request.Title.Trim();
        book.Description = request.Description?.Trim();
        book.FileStorageKey = request.FileStorageKey.Trim();

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}