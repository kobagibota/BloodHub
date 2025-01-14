using BloodHub.Shared.Extentions;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace BloodHub.Shared.Entities
{
    public class AuthToken : BaseEntity
    {
        public int UserId { get; set; }
        public required string Token { get; set; } = string.Empty; // Giá trị token
        public TokenType TokenType { get; set; } = TokenType.RefreshToken; // Loại token
        public DateTime ExpiryDate { get; set; } // Thời gian hết hạn
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Ngày tạo token
        public DateTime? RevokedAt { get; set; } // Ngày bị thu hồi (nếu có)
        public string? ReplacedByToken { get; set; } // Token mới thay thế token này
        public bool IsUsed { get; set; } = false; // Token đã được sử dụng chưa

        // Trạng thái token
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate; // Token hết hạn
        public bool IsActive => !IsUsed && !IsExpired && RevokedAt == null; // Token còn hoạt động

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
