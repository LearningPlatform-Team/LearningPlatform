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
    }
}
