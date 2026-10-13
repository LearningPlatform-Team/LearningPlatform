using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Note.Command.UpdateNote
{
    public class PatchNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        : IRequestHandler<PatchNoteCommand, Result>
    {
        public async Task<Result> Handle(PatchNoteCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var note = await context.Notes
                .FirstOrDefaultAsync(n => n.Id == request.Id && n.UserId == userId, cancellationToken);

            if (note is null)
                return Result.Failure("Note not found.", ErrorType.NotFound);

            if (request.Title is not null)
                note.Title = request.Title.Trim();

            if (request.Content is not null)
            {
                var content = request.Content.Trim();
                note.Content = content.Length == 0 ? null : content;
            }

            if (request.Color.HasValue)
                note.Color = request.Color.Value;

            note.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
