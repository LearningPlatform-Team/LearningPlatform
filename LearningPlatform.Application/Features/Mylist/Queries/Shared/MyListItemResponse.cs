using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.Mylist.Queries.Shared
{
    public record MyListItemResponse(
        Guid LectureId,
        string Title,
        string Description,
        int DurationMinutes,
        Guid SubjectOfferingId,
        string SubjectName);
}
