using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades
{
    public record MyGradeResponse(
      string SubjectName,
      decimal Score,
      decimal MaxScore,
      DateTime? ExamDate,
      DateTime GradedAt,
      Guid? GradedByUserId);
}
