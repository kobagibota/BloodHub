using BloodHub.Shared.Extentions;

namespace BloodHub.Shared.DTOs
{
    public class ShiftDto
    {
        public int Id { get; set; }

        public string ShiftName { get; set; } = string.Empty;

        public DateTime ShiftStart { get; set; }

        public DateTime? ShiftEnd { get; set; }

        public DateTime? HandoverTime { get; set; }

        public int? ReceivedShiftId { get; set; }

        public int? HandBy { get; set; }
        public string? UserHand { get; set; }

        public int? ReceivedBy { get; set; }
        public string? UserReceived { get; set; }

        public string? Note { get; set; }

        public ShiftStatus Status { get; set; }

        public List<ShiftUserDto> ShiftUsers { get; set; } = new List<ShiftUserDto>();
    }
}
