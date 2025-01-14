namespace BloodHub.Shared.Request
{
    public class DoctorRequest
    {
        public required string DoctorName { get; set; }

        public bool IsHide { get; set; }
    }
}
