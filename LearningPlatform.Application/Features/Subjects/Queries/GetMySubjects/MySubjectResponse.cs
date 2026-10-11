namespace LearningPlatform.Application.Features.Subjects.Queries.GetMySubjects;
public record MySubjectResponse(
    Guid EnrollmentId,
    Guid OfferingId,
    Guid SubjectId,
    string Code,
    string Name,
    string? Description,
    string YearLevel,
    string AcademicYear,
    int TermNumber,
    DateTime EnrolledAt
);


