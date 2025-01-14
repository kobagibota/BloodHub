using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Doctor : BaseEntity
    {
        [Required, MaxLength(100)]
        public string DoctorName { get; set; } = string.Empty;
        public bool IsHide { get; set; } = false;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
