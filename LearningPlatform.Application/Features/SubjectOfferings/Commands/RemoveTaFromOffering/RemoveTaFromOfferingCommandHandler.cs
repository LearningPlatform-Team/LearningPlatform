using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.RemoveTaFromOffering;

public sealed class RemoveTaFromOfferingCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<RemoveTaFromOfferingCommand, Result>
{
    public async Task<Result> Handle(
        RemoveTaFromOfferingCommand request,
        CancellationToken cancellationToken)
    {
        var offeringExists = await context.SubjectOfferings
            .AnyAsync(
                x => x.Id == request.SubjectOfferingId,
                cancellationToken);

        if (!offeringExists)
        {
            return Result.Failure(
                "Subject offering was not found.",
                ErrorType.NotFound);
        }

        var assignment = await context.OfferingTas
            .FirstOrDefaultAsync(
                x => x.OfferingId == request.SubjectOfferingId
                     && x.TaProfileId == request.TaProfileId,
                cancellationToken);

        if (assignment is null)
        {
            return Result.Failure(
                "TA assignment was not found.",
                ErrorType.NotFound);
        }

        context.OfferingTas.Remove(assignment);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}