using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Commands.UpdateLecture;

public sealed class UpdateLectureCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<UpdateLectureCommand, Result>
{
    public async Task<Result> Handle(
        UpdateLectureCommand request,
        CancellationToken cancellationToken)
    {
        var lecture = await context.Lectures
            .FirstOrDefaultAsync(
                x => x.Id == request.Id,
                cancellationToken);

        if (lecture is null)
        {
            return Result.Failure(
                "Lecture was not found.",
                ErrorType.NotFound);
        }

        var offeringExists = await context.SubjectOfferings
            .AnyAsync(
                x => x.Id == request.SubjectOfferingId,
                cancellationToken);

        if (!offeringExists)
        {
            return Result.Failure(
                "Subject offering was not found.",
                ErrorType.NotFound);
        }

        lecture.SubjectOfferingId = request.SubjectOfferingId;
        lecture.Title = request.Title.Trim();
        lecture.Description = request.Description.Trim();
        lecture.DurationMinutes = request.DurationMinutes;
        lecture.VideoProvider = request.VideoProvider.Trim();
        lecture.VideoUrl = request.VideoUrl.Trim();
        lecture.SortOrder = request.SortOrder;
        lecture.IsPublished = request.IsPublished;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}