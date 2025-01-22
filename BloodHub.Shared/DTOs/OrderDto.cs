namespace BloodHub.Shared.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public int WardId { get; set; }
        public string WardName { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public string Diagnosis { get; set; } = string.Empty;

        public string? Room { get; set; }
    }
}
