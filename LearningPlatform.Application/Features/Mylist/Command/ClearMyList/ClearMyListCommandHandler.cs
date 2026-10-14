using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Mylist.Command.ClearMyList
{
    public class ClearMyListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
      : IRequestHandler<ClearMyListCommand, Result>
    {
        public async Task<Result> Handle(ClearMyListCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var deletedCount = await context.MyListItems
                 .Where(m => m.UserId == userId)
                 .ExecuteDeleteAsync(cancellationToken);

            if (deletedCount == 0)
                return Result.Failure("Your list is already empty.", ErrorType.NotFound);

            return Result.Success();
        }
    }
}
