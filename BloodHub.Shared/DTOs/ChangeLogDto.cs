using BloodHub.Shared.Extentions;

namespace BloodHub.Shared.DTOs
{
    public class ChangeLogDto
    {
        public int Id { get; set; }

        public string? UserName { get; set; } // Người dùng thực hiện thao tác

        public ChangeType ChangeType { get; set; } // Enum đại diện cho hành động

        public required string TableName { get; set; }

        public int RecordId { get; set; }

        public List<ChangeLogDetail> ChangeDetails { get; set; } = new();

        public DateTime ChangeTimestamp { get; set; }

        public string IpAddress { get; set; } = string.Empty;         // Địa chỉ IP của người dùng

        public string UserAgent { get; set; } = string.Empty;       // Thông tin thiết bị/người dùng
    }
}
