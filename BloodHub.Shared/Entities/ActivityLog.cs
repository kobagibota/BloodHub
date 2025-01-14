using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class ActivityLog : BaseEntity
    {
        public int UserId { get; set; } // Người dùng thực hiện thao tác
        public Activity Activity { get; set; } // Enum đại diện cho hành động

        [Column(TypeName = "nvarchar(max)")]
        public string Token { get; set; } = string.Empty; // Token liên quan
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Thời gian thực hiện
        
        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;         // Địa chỉ IP của người dùng
        [MaxLength(1024)]
        public string UserAgent { get; set; } = string.Empty;       // Thông tin thiết bị/người dùng

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
