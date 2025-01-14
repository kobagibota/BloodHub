using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class ChangeLog : BaseEntity
    {
        [Required]
        public int UserId { get; set; } // Người dùng thực hiện thao tác
        
        public ChangeType ChangeType { get; set; } // Enum đại diện cho hành động
        
        [Required, MaxLength(50)] 
        public required string TableName { get; set; }
        
        [Required] 
        public int RecordId { get; set; }              
        
        [MaxLength(int.MaxValue)] 
        public string? ChangeDetails { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] 
        public DateTime ChangeTimestamp { get; set; }

        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;         // Địa chỉ IP của người dùng
        
        [MaxLength(1024)]
        public string UserAgent { get; set; } = string.Empty;       // Thông tin thiết bị/người dùng

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
