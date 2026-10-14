using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;

public sealed class CreateLectureCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<CreateLectureCommand, Result>
{
    public async Task<Result> Handle(
        CreateLectureCommand request,
        CancellationToken cancellationToken)
    {
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
        var uploadedById = Guid.Empty;
        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            SubjectOfferingId = request.SubjectOfferingId,
            UploadedById = uploadedById,
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