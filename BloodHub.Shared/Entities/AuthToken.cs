using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class AuthToken : BaseEntity
    {
        public int UserId { get; set; }

        [Required, MaxLength(500)]
        public string TokenHash { get; set; } = string.Empty; // Lưu hash thay vì token thô

        public TokenType TokenType { get; set; } = TokenType.RefreshToken; // Loại token (Access, Refresh,...)

        public DateTime ExpiryDate { get; set; } // Thời gian hết hạn

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo token

        public DateTime? RevokedAt { get; set; } // Ngày bị thu hồi (nếu có)

        public bool RevokedByAdmin { get; set; } = false; // Bị thu hồi do admin không?

        public string? ReplacedByToken { get; set; } // Token mới thay thế token này

        public bool IsUsed { get; set; } = false; // Token đã được sử dụng chưa

        public string? IpAddress { get; set; } // Địa chỉ IP của thiết bị sử dụng token

        public string? DeviceInfo { get; set; } // Thông tin thiết bị (User-Agent, OS,...)

        // Trạng thái token
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate; // Token hết hạn
        public bool IsActive => !IsUsed && !IsExpired && RevokedAt == null; // Token còn hoạt động

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
