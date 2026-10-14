using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Books.Commands.CreateBook;

public sealed class CreateBookCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateBookCommand, Result>
{
    public async Task<Result> Handle(
        CreateBookCommand request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        if (userId is null || userId == Guid.Empty)
        {
            return Result.Failure(
                "User is not authenticated.",
                ErrorType.Unauthorized);
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

        var book = new Book
        {
            Id = Guid.NewGuid(),
            SubjectId = request.SubjectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            FileStorageKey = request.FileStorageKey.Trim(),
            UploadedByUserId = userId.Value
        };

        context.Books.Add(book);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}