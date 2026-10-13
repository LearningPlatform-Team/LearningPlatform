using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.MyGrades.Queries.GetMyGrades
{
    public class GetMyGradesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
           : IRequestHandler<GetMyGradesQuery, ResultT<List<MyGradeResponse>>>
    {
        public async Task<ResultT<List<MyGradeResponse>>> Handle(GetMyGradesQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<List<MyGradeResponse>>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var query = context.Grades
                .AsNoTracking()
                .Where(g => g.Enrollment.StudentId == userId
                         && g.Enrollment.Status != EnrollmentStatus.Dropped);

            if (request.TermId.HasValue)
                query = query.Where(g => g.Enrollment.SubjectOffering.TermId == request.TermId.Value);

            var grades = await query
                .OrderBy(g => g.Enrollment.SubjectOffering.Subject.Name)
                .ThenBy(g => g.Assessment.ExamDate)
                .Select(g => new MyGradeResponse(
                    g.Enrollment.SubjectOffering.Subject.Name,
                    g.Score,
                    g.Assessment.MaxScore,
                    g.Assessment.ExamDate,
                    g.GradedAt,
                    g.GradedByUserId))
                .ToListAsync(cancellationToken);

            return ResultT<List<MyGradeResponse>>.Success(grades);
        }
    }
}
