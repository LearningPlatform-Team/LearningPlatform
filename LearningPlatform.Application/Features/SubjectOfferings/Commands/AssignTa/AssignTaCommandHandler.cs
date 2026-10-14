using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Academic;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.SubjectOfferings.Commands.AssignTa;

public class AssignTaCommandHandler(
    IApplicationDbContext context)
    : IRequestHandler<AssignTaCommand, Result>
{
    public async Task<Result> Handle(
        AssignTaCommand request,
        CancellationToken cancellationToken)
    {
        var offeringExists = await context.SubjectOfferings
            .AnyAsync(
                o => o.Id == request.OfferingId,
                cancellationToken);

        if (!offeringExists)
        {
            return Result.Failure(
                "Subject offering not found.",
                ErrorType.NotFound);
        }

        var taExists = await context.TaProfiles
            .AnyAsync(
                ta => ta.UserId == request.TaProfileId,
                cancellationToken);

        if (!taExists)
        {
            return Result.Failure(
                "TA profile not found.",
                ErrorType.NotFound);
        }

        var alreadyAssigned = await context.OfferingTas
            .AnyAsync(
                ot => ot.OfferingId == request.OfferingId
                   && ot.TaProfileId == request.TaProfileId,
                cancellationToken);

        if (alreadyAssigned)
        {
            return Result.Failure(
                "This TA is already assigned to this subject offering.",
                ErrorType.Conflict);
        }

        var offeringTa = new OfferingTa
        {
            OfferingId = request.OfferingId,
            TaProfileId = request.TaProfileId,
            AssignedAt = DateTime.UtcNow
        };

        context.OfferingTas.Add(offeringTa);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}