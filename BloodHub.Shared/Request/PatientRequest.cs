using BloodHub.Shared.Extentions;
namespace BloodHub.Shared.Request
{
    public class PatientRequest
    {
        public string? MedicalId { get; set; }

        public string PatientName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Age
        {
            get
            {
                var today = DateTime.Today;
                var ageInDays = (today - DateOfBirth).Days;
                if (ageInDays < 30)
                {
                    return $"{ageInDays} ngày";
                }

                var ageInMonths = ageInDays / 30;       // Giả sử mỗi tháng có 30 ngày
                if (ageInMonths < 72)
                {
                    return $"{ageInMonths} tháng";
                }

                var ageInYears = today.Year - DateOfBirth.Year;
                if (DateOfBirth > today.AddYears(-ageInYears))
                {
                    ageInYears--;
                }
                return $"{ageInYears} tuổi";
            }
        }

        public Gender Gender { get; set; }

        public string? Address { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public Rhesus Rhesus { get; set; }
    }
}
