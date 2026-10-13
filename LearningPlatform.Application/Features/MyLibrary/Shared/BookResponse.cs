using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.MyLibrary.Shared
{
    public record BookResponse(
        Guid Id,
        string Title,
        string? Description,
        Guid? SubjectId,
        string? SubjectName);

}
