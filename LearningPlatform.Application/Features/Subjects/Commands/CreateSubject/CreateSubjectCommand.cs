using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Commands.CreateSubject;

public record CreateSubjectCommand(
     string Code,
     string Name,
     string? Description,
     YearLevel YearLevel) : IRequest<Result>;

