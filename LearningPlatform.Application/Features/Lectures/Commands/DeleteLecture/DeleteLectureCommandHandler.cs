using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Lectures.Commands.DeleteLecture;

public sealed class DeleteLectureCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<DeleteLectureCommand, Result>
{
    public async Task<Result> Handle(
        DeleteLectureCommand request,
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

        context.Lectures.Remove(lecture);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}