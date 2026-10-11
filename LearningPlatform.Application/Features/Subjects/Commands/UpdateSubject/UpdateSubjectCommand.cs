using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Commands.UpdateSubject;

public record UpdateSubjectCommand(
 Guid Id,
 string Code,
 string Name,
 string? Description,
 YearLevel YearLevel
) : IRequest<Result>;
