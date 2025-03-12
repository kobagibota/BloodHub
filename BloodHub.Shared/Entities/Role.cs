using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Role : BaseEntity
    {
        [Required, MaxLength(100)]
        public required string Name { get; set; } = null!;

        // Liên kết với bảng trung gian UserRoles
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
