using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetMyEnrollmentHistory;

public record EnrollmentHistoryResponse(
    Guid EnrollmentId,
    Guid OfferingId,
    string SubjectCode,
    string SubjectName,
    EnrollmentStatus Status,
    DateTime EnrolledAt
);
