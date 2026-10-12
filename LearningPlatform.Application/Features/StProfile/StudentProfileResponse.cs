using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningPlatform.Domain.Enums;

namespace LearningPlatform.Application.Features.StProfile
{
    public record StudentProfileResponse(
       Guid Id,
       string FullName,
       string? Email,
       string? PhoneNumber,
       string AvatarUrl,
       AppLanguage PreferredLanguage,
       string StudentCode);
}
