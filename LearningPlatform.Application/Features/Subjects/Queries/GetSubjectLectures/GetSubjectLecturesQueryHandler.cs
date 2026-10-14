using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Lectures.Shared;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectLectures;

public sealed class GetSubjectLecturesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSubjectLecturesQuery, ResultT<List<LectureResponse>>>
{
    public async Task<ResultT<List<LectureResponse>>> Handle(
        GetSubjectLecturesQuery request,
        CancellationToken cancellationToken)
    {
        var studentId = currentUserService.UserId;

        if (studentId is null || studentId == Guid.Empty)
        {
            return ResultT<List<LectureResponse>>.Failure(
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
            return ResultT<List<LectureResponse>>.Failure(
                "You are not enrolled in this subject.",
                ErrorType.Conflict);
        }

        var lectures = await context.Lectures
            .AsNoTracking()
            .Where(l =>
                l.Offering.SubjectId == request.SubjectId &&
                l.IsPublished)
            .OrderBy(l => l.SortOrder)
            .Select(l => new LectureResponse(
                l.Id,
                l.SubjectOfferingId,
                l.UploadedById,
                l.Title,
                l.Description,
                l.DurationMinutes,
                l.VideoProvider,
                l.VideoUrl,
                l.SortOrder,
                l.CreatedAt,
                l.IsPublished))
            .ToListAsync(cancellationToken);

        return ResultT<List<LectureResponse>>.Success(lectures);
    }
}