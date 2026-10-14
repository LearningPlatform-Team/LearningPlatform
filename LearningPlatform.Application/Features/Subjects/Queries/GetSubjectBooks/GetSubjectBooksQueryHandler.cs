using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Books.Shared;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectBooks;

public sealed class GetSubjectBooksQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSubjectBooksQuery, ResultT<List<BookResponse>>>
{
    public async Task<ResultT<List<BookResponse>>> Handle(
        GetSubjectBooksQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = currentUserService.UserId;

        if (studentId is null || studentId == Guid.Empty)
        {
            return ResultT<List<BookResponse>>.Failure(
                "User is not authenticated.",
                ErrorType.Unauthorized);
        }

        var isEnrolled = await context.Enrollments
            .AnyAsync(e =>
                e.StudentId == studentId.Value &&
                e.Status == EnrollmentStatus.Enrolled &&
                e.SubjectOffering.SubjectId == request.SubjectId,
                cancellationToken);

        if (!isEnrolled)
        {
            return ResultT<List<BookResponse>>.Failure(
                "You are not enrolled in this subject.",
                ErrorType.Conflict);
        }

        var books = await context.Books
            .AsNoTracking()
            .Where(b => b.SubjectId == request.SubjectId)
            .OrderBy(b => b.Title)
            .Select(b => new BookResponse(
                b.Id,
                b.SubjectId,
                b.Title,
                b.Description,
                b.FileStorageKey,
                b.UploadedByUserId))
            .ToListAsync(cancellationToken);

        return ResultT<List<BookResponse>>.Success(books);
    }
}