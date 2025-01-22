namespace BloodHub.Shared.Request
{
    public class OrderRequest
    {
        public int PatientId { get; set; }

        public int WardId { get; set; }

        public int DoctorId { get; set; }

        public DateTime OrderDate { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string? Room { get; set; }
    }
}
