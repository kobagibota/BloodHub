namespace BloodHub.Shared.Request
{
    public class ShiftConfirmHandoverRequest
    {
        public DateTime? HandoverTime { get; set; }

        public int? ReceivedBy { get; set; }

    }
}
