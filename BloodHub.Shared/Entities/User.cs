using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class User : IdentityUser<int>
    {
        // Thuộc tính riêng
        [Required, MaxLength(150)]
        public required string FullName { get; set; }

        public string ShortName
        {
            get
            {
                if (string.IsNullOrEmpty(FullName))
                    return string.Empty;

                // Tách các phần của tên  
                var parts = FullName.Split('.');
                if (parts.Length < 2)
                    return FullName;

                // Lấy chức danh và tên  
                var title = parts[0].Trim();
                var nameParts = parts[1].Trim().Split(' ');

                // Trả về chức danh và tên cuối cùng  
                return $"{title}. {nameParts[nameParts.Length - 1]}";
            }
        }

        public bool IsActive { get; set; }

        [MaxLength(255)]
        public string? ContactInfo { get; set; }

        public DateTime CreatedAt { get; set; }

        // Quan hệ với các bảng khác
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<AuthToken> RefreshTokens { get; set; } = new List<AuthToken>();

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