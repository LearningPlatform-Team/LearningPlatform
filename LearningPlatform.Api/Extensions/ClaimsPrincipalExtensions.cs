using LearningPlatform.Domain.Entities.Identity;
using System.Security.Claims;

namespace LearningPlatform.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
            {
                return userId;
            }
            else
            {
                throw new UnauthorizedAccessException("The authenticated user ID is missing or invalid.");
            }
        }
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole("Admin");
        }
        public static bool IsDoctor(this ClaimsPrincipal user)
        {
            return user.IsInRole("Doctor");
        }
        public static bool IsTA(this ClaimsPrincipal user)
        {
            return user.IsInRole("TA");
        }
    }
}
