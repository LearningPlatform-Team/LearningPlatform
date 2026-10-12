using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.StProfile
{
    public class GetStudentProfileQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
          : IRequestHandler<GetStudentProfileQuery, ResultT<StudentProfileResponse>>
    {
        public async Task<ResultT<StudentProfileResponse>> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
        {
            if (currentUser.UserId is not Guid userId)
                return ResultT<StudentProfileResponse>.Failure("User is not authenticated.", ErrorType.Unauthorized);

            var profile = await context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && u.StudentProfile != null)
                .Select(u => new StudentProfileResponse(
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.PhoneNumber,
                    u.AvatarUrl,
                    u.PreferredLanguage,
                    u.StudentProfile!.StudentCode))
                .FirstOrDefaultAsync(cancellationToken);

            if (profile is null)
                return ResultT<StudentProfileResponse>.Failure("Student profile not found.", ErrorType.NotFound);

            return ResultT<StudentProfileResponse>.Success(profile);
        }
    }
}
