using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Patient : BaseEntity
    {
        [MaxLength(50)]
        public string? MedicalId { get; set; }

        [Required, MaxLength(200)]
        public string PatientName { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string Age
        {
            get
            {
                if (DateOfBirth == DateTime.MinValue)
                {
                    return string.Empty;
                }

                var today = DateTime.Today;
                var ageInDays = (today - DateOfBirth).Days;
                if (ageInDays < 30)
                {
                    return $"{ageInDays} ngày";
                }

                var ageInMonths = ageInDays / 30;
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

        [MaxLength(500)]
        public string? Address { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public Rhesus Rhesus { get; set; }

        // Thuộc tính chỉ để đọc kết hợp BloodGroup và Rhesus
        public string BloodGroupDescription 
        { 
            get 
            { 
                string rhesusSymbol = Rhesus == Rhesus.Positive ? "Rh(+)" : "Rh(-)"; 
                return $"{BloodGroup}, {rhesusSymbol}"; 
            } 
        }

        public string BloodGroupColor => BloodGroupColorMapping.GetColor(BloodGroup); 
        public string RhesusColor => RhesusColorMapping.GetColor(Rhesus);

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
