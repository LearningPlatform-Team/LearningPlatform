using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.MyLibrary.Shared;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.MyLibrary.Quiers.GetBookById
{
    public class GetBookByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
            : IRequestHandler<GetBookByIdQuery, ResultT<BookResponse>>
    {
        public async Task<ResultT<BookResponse>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<BookResponse>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var enrolledSubjectIds = context.Enrollments
                .AsNoTracking()
                .Where(en => en.StudentId == userId && en.Status == EnrollmentStatus.Enrolled)
                .Select(en => en.SubjectOffering.SubjectId);

            var book = await context.Books
                .AsNoTracking()
                .Where(b => b.Id == request.Id
                         && (b.SubjectId == null || enrolledSubjectIds.Contains(b.SubjectId.Value)))
                .Select(b => new BookResponse(
                    b.Id,
                    b.Title,
                    b.Description,
                    b.SubjectId,
                    b.Subject != null ? b.Subject.Name : null))
                .FirstOrDefaultAsync(cancellationToken);

            if (book is null)
                return ResultT<BookResponse>.Failure("Book not found.", ErrorType.NotFound);

            return ResultT<BookResponse>.Success(book);
        }
    }
}
