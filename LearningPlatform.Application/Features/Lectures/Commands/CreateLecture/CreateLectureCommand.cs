using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Lectures.Commands.CreateLecture;

public sealed record CreateLectureCommand(
    Guid SubjectOfferingId,
    string Title,
    string Description,
    int DurationMinutes,
    string VideoProvider,
    string VideoUrl,
    int SortOrder,
    bool IsPublished
) : IRequest<Result>;