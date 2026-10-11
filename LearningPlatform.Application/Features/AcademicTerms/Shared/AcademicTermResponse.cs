namespace LearningPlatform.Application.Features.AcademicTerms.Shared;
public record AcademicTermResponse(
    Guid Id,
    string AcademicYear,
    int TermNumber,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsCurrent
);
