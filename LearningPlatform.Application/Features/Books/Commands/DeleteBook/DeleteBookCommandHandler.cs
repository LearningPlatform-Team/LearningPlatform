using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Books.Commands.DeleteBook;

public sealed class DeleteBookCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteBookCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBookCommand request,
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

        context.Books.Remove(book);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}