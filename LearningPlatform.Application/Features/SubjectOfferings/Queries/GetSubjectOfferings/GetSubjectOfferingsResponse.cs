using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.SubjectOfferings.Queries.GetSubjectOfferings;

public record SubjectOfferingResponse(
    Guid Id,
    Guid SubjectId,
    string SubjectCode,
    string SubjectName,
    YearLevel YearLevel,
    Guid TermId,
    Guid DoctorProfileId,
    bool IsActive
);