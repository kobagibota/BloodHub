namespace BloodHub.Shared.Request
{
    public class ShiftRequest
    {
        public int Id { get; set; }

        public string ShiftName { get; set; } = string.Empty;

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public List<int> UserIds { get; set; } = new();
    }
}
