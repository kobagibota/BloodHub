namespace BloodHub.Shared.Request
{
    public class ShiftHandoverRequest
    {
        public DateTime ShiftEnd { get; set; }

        public int ReceivedShiftId { get; set; }

        public int HandBy { get; set; }

        public string? Note { get; set; }
    }
}
