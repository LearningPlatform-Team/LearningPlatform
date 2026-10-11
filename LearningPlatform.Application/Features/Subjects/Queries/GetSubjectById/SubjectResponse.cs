namespace LearningPlatform.Application.Features.Subjects.Queries.GetSubjectById;

public record SubjectResponse(
 Guid Id,
 string Code,
 string Name,
 string? Description,
 string YearLevel);
