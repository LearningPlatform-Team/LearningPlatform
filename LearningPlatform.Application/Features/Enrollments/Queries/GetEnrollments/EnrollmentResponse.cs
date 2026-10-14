using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Enrollments.Queries.GetEnrollments;

public record EnrollmentResponse(
    Guid Id,
    Guid StudentId,
    Guid OfferingId,
    Guid EnrolledBy,
    DateTime EnrolledAt,
    EnrollmentStatus Status
);