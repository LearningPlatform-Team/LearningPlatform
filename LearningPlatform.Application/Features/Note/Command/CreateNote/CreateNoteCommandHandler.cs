using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Note.Command.CreateNote
{
    public class CreateNoteCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser) : IRequestHandler<CreateNoteCommand, Result>
    {
        public async Task<Result> Handle(CreateNoteCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.",ErrorType.Unauthorized);

            var userExists = await context.Users.AnyAsync(u => u.Id == userId, cancellationToken);

            if (!userExists)
                return Result.Failure("User not found.",ErrorType.NotFound);

            var note = new Domain.Entities.Content.Note
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = request.Title.Trim(),
                Content = request.Content?.Trim(),
                Color = request.Color,
                UpdatedAt = DateTime.UtcNow
            };

            context.Notes.Add(note);
            await context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
