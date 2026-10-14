using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Note.Queries.GetNoteByIdQuery
{
    public class GetNoteByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser) : IRequestHandler<GetNoteByIdQuery, ResultT<NoteResponse>>
    {
        public async Task<ResultT<NoteResponse>> Handle(GetNoteByIdQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<NoteResponse>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var note = await context.Notes
                .AsNoTracking()
                .Where(n => n.Id == request.Id && n.UserId == userId)
                .Select(n => new NoteResponse(n.Id, n.Title, n.Content, n.Color, n.UpdatedAt))
                .FirstOrDefaultAsync(cancellationToken);

            if (note is null)
                return ResultT<NoteResponse>.Failure("Note not found.", ErrorType.NotFound);

            return ResultT<NoteResponse>.Success(note);
        }
    }
}
