using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;

namespace LearningPlatform.Application.Features.Note.Command.UpdateNote
{
    public record PatchNoteCommand(Guid Id, string? Title, string? Content, NoteColor? Color) : IRequest<Result>;
}
