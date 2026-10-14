using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;

public sealed class CreateLectureCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateLectureCommand, Result>
{
    public async Task<Result> Handle(
        CreateLectureCommand request,
        CancellationToken cancellationToken)
    {
        var uploadedById = currentUserService.UserId;

        if (uploadedById is null || uploadedById == Guid.Empty)
        {
            return Result.Failure(
                "User is not authenticated.",
                ErrorType.Unauthorized);
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

        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            SubjectOfferingId = request.SubjectOfferingId,
            UploadedById = uploadedById.Value,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            DurationMinutes = request.DurationMinutes,
            VideoProvider = request.VideoProvider.Trim(),
            VideoUrl = request.VideoUrl.Trim(),
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            IsPublished = request.IsPublished
        };

        context.Lectures.Add(lecture);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}