using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Note.Queries.GetNoteByIdQuery
{
    public record GetNoteByIdQuery(Guid Id) : IRequest<ResultT<NoteResponse>>;
}
