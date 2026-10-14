using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.Note.Queries
{
    public record NoteResponse(
          Guid Id,
          string Title,
          string? Content,
          NoteColor Color,
          DateTime UpdatedAt);

}
