using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Domain.Entities.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
