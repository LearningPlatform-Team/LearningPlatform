using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Note.Command.UpdateNote
{
    public record PatchNoteRequest(string? Title, string? Content, NoteColor? Color);
}
