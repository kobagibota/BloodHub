using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class Order : BaseEntity
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        public DateTime OrderDate { get; set; }

        [MaxLength(500)]
        public string Diagnosis { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Room { get; set; }

        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; } = null!;

        [ForeignKey(nameof(WardId))]
        public virtual Ward Ward { get; set; } = null!;

        [ForeignKey(nameof(DoctorId))]
        public virtual Doctor Doctor { get; set; } = null!;

        public virtual ICollection<Crossmatch> Crossmatches { get; set; } = new List<Crossmatch>();
    }
}
