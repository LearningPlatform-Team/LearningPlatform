using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Subjects.Queries.GetMySubjects;

public class GetMySubjectsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMySubjectsQuery, ResultT<List<MySubjectResponse>>>
{
    public async Task<ResultT<List<MySubjectResponse>>> Handle(
        GetMySubjectsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not Guid userId)
        {
            return ResultT<List<MySubjectResponse>>.Failure(
                "User is not authenticated.",
                ErrorType.Unauthorized);
        }

        var subjects = await context.Enrollments
            .AsNoTracking()
            .Where(e =>
                e.StudentId == userId &&
                e.Status == EnrollmentStatus.Enrolled &&
                e.SubjectOffering.Term.IsCurrent)
            .Select(e => new MySubjectResponse(
                e.Id,
                e.OfferingId,
                e.SubjectOffering.Subject.Id,
                e.SubjectOffering.Subject.Code,
                e.SubjectOffering.Subject.Name,
                e.SubjectOffering.Subject.Description,
                e.SubjectOffering.Subject.YearLevel.ToString(),
                e.SubjectOffering.Term.AcademicYear,
                e.SubjectOffering.Term.TermNumber,
                e.EnrolledAt))
            .ToListAsync(cancellationToken);

        return ResultT<List<MySubjectResponse>>.Success(subjects);
    }
}