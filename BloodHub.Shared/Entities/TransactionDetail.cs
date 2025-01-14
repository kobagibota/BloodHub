using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class TransactionDetail : BaseEntity
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public int TransactionId { get; set; }

        public TransactionStatus Status { get; set; }

        [ForeignKey(nameof(InventoryId))]
        public virtual Inventory Inventory { get; set; } = null!;

        [ForeignKey(nameof(TransactionId))]
        public virtual Transaction Transaction { get; set; } = null!;
    }
}
