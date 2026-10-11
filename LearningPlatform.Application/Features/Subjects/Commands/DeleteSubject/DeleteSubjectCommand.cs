using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Subjects.Commands.DeleteSubject;

public record DeleteSubjectCommand(Guid Id) : IRequest<Result>;
