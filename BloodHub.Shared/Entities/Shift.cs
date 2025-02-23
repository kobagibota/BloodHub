using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class Shift : BaseEntity
    {
        [Required, MaxLength(250)]
        public string ShiftName { get; set; } = string.Empty;

        [Required]
        public DateTime ShiftStart { get; set; }

        public DateTime? ShiftEnd { get; set; }

        public DateTime? HandoverTime { get; set; }

        public int? ReceivedShiftId { get; set; }

        public int? HandBy { get; set; }

        public int? ReceivedBy { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }

        public ShiftStatus Status { get; set; }

        [ForeignKey(nameof(ReceivedShiftId))]
        public Shift? ReceivedShift { get; set; } = null!;

        [ForeignKey(nameof(HandBy))]
        [InverseProperty("ShiftsHanded")] 
        public User? UserHand { get; set; } = null; 
        
        [ForeignKey(nameof(ReceivedBy))]
        [InverseProperty("ShiftsReceived")]
        public User? UserReceived { get; set; } = null;

        public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public virtual ICollection<ShiftUser> ShiftUsers { get; set; } = new List<ShiftUser>();

    }
}
