using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class User : BaseEntity
    {
        [Required, MaxLength(50)]
        public required string Username { get; set; }

        [Required, MaxLength(int.MaxValue)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public required string Title { get; set; }          // chức danh

        [Required, MaxLength(50)]
        public required string FirstName { get; set; }      // Tên đệm

        [Required, MaxLength(150)]
        public required string LastName { get; set; }       // Họ

        public string FullName => $"{Title}. {LastName} {FirstName}";

        public string ShortName => $"{Title}. {FirstName}";

        [MaxLength(255)]
        public string? ContactInfo { get; set; }

        public bool IsActive { get; set; }

        public bool IsOnDuty { get; set; }      // Tham gia trực

        public bool MustChangePassword { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        // Quan hệ với các bảng khác
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<AuthToken> AuthTokens { get; set; } = new List<AuthToken>();

        public virtual ICollection<ShiftUser> ShiftUsers { get; set; } = new List<ShiftUser>();
        public virtual ICollection<Shift> Shifts { get; set; } = new List<Shift>();
        [InverseProperty("UserHand")]
        public virtual ICollection<Shift> ShiftsHanded { get; set; } = new List<Shift>();
        [InverseProperty("UserReceived")]
        public virtual ICollection<Shift> ShiftsReceived { get; set; } = new List<Shift>();
        public virtual ICollection<Crossmatch> Crossmatches { get; set; } = new List<Crossmatch>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}