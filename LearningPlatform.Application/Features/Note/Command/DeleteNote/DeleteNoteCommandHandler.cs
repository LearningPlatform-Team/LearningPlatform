using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Note.Command.DeleteNote
{
    public class DeleteNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
           : IRequestHandler<DeleteNoteCommand, Result>
    {
        public async Task<Result> Handle(DeleteNoteCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, cancellationToken);

            if (note is null)
                return Result.Failure("Note not found.", ErrorType.NotFound);

            context.Notes.Remove(note);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
