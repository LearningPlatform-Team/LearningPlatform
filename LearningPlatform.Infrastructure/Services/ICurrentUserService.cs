using System.Security.Claims;
using LearningPlatform.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LearningPlatform.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public Guid? UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(userId, out var id) ? id : null;
            }
        }
    }
}
