using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Mylist.Command.RemoveFromList
{
    public class RemoveFromMyListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
      : IRequestHandler<RemoveFromMyListCommand, Result>
    {
        public async Task<Result> Handle(RemoveFromMyListCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var item = await context.MyListItems
                .FirstOrDefaultAsync(m => m.UserId == userId && m.LectureId == request.LectureId, cancellationToken);

            if (item is null)
                return Result.Failure("Lecture is not in your list.", ErrorType.NotFound);

            context.MyListItems.Remove(item);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
