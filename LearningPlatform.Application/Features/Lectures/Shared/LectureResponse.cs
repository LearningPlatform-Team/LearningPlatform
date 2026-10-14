namespace LearningPlatform.Application.Features.Lectures.Shared
{
    public sealed record LectureResponse(
        Guid Id,
        Guid SubjectOfferingId,
        Guid UploadedById,
        string Title,
        string Description,
        int DurationMinutes,
        string VideoProvider,
        string VideoUrl,
        int SortOrder,
        DateTime CreatedAt,
        bool IsPublished
    );
}
