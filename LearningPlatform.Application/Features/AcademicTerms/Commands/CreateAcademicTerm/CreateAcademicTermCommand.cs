using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.AcademicTerms.Commands.CreateAcademicTerm;

public record CreateAcademicTermCommand(
    string AcademicYear,
    int TermNumber,
    DateOnly StartDate,
    DateOnly EndDate
) : IRequest<Result>;