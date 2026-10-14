using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.DeleteAcademicTerm;

public sealed record DeleteAcademicTermCommand(Guid Id) : IRequest<Result>;
