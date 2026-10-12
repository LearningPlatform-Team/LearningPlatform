using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities.Content;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Mylist.Command.CreateList
{
    public class AddToMyListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
         : IRequestHandler<AddToMyListCommand, Result>
    {
        public async Task<Result> Handle(AddToMyListCommand request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return Result.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var lecture = await context.Lectures
                .AsNoTracking()
                .Where(l => l.Id == request.LectureId)
                .Select(l => new { l.SubjectOfferingId, l.IsPublished })
                .FirstOrDefaultAsync(cancellationToken);

            if (lecture is null || !lecture.IsPublished)
                return Result.Failure("Lecture not found.", ErrorType.NotFound);

            var isEnrolled = await context.Enrollments
                .AnyAsync(en => en.StudentId == userId
                             && en.OfferingId == lecture.SubjectOfferingId
                             && en.Status == EnrollmentStatus.Enrolled,
                          cancellationToken);

            if (!isEnrolled)
                return Result.Failure("Lecture not found.", ErrorType.NotFound);

            var alreadyInList = await context.MyListItems
                .AnyAsync(m => m.UserId == userId && m.LectureId == request.LectureId, cancellationToken);

            if (alreadyInList)
                return Result.Failure("Lecture is already in your list.", ErrorType.Conflict);

            context.MyListItems.Add(new MyListItem
            {
                UserId = userId,
                LectureId = request.LectureId
            });

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {

                return Result.Failure("Lecture is already in your list.", ErrorType.Conflict);
            }

            return Result.Success();
        }
    }
}
