using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Domain.Entities.Identity
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
