using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Lectures.Commands.UpdateLecture;

public sealed record UpdateLectureCommand(
    Guid Id,
    Guid SubjectOfferingId,
    string Title,
    string Description,
    int DurationMinutes,
    string VideoProvider,
    string VideoUrl,
    int SortOrder,
    bool IsPublished
) : IRequest<Result>;
