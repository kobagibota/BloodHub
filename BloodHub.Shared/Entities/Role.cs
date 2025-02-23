using Microsoft.AspNetCore.Identity;

namespace BloodHub.Shared.Entities
{
    public class Role : IdentityRole<int>
    {
        // Liên kết với bảng trung gian UserRoles
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
