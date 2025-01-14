using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class Transaction : BaseEntity
    {
        [Required]
        public int CrossmatchId { get; set; }

        public TransactionType TransactionType { get; set; }

        public DateTime TracsactionDate { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
        
        [Required]
        public int ProcessedBy { get; set; }

        [Required]
        public int NurseId { get; set; }
        
        [Required]                
        public int ShiftId { get; set; }

        public TransactionStatus Status { get; set; }


        [ForeignKey(nameof(CrossmatchId))]
        public virtual Crossmatch Crossmatch { get; set; } = null!;

        [ForeignKey(nameof(ProcessedBy))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(NurseId))]
        public virtual Nursing Nursing { get; set; } = null!;

        [ForeignKey(nameof(ShiftId))]
        public virtual Shift Shift { get; set; } = null!;

        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();
    }
}
