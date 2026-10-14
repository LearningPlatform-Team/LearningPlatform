namespace LearningPlatform.Application.Features.Books.Shared;


public record BookResponse(
    Guid Id,
    Guid? SubjectId,
    string Title,
    string? Description,
    string FileStorageKey,
    Guid UploadedByUserId
);
