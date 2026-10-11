using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.UpdateAcademicTerm;

public record UpdateAcademicTermCommand(
    Guid Id,
    string AcademicYear,
    int TermNumber,
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest<Result>;