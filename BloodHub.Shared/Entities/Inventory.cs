using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class Inventory : BaseEntity
    {
        [Required]
        public int ProductId { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public Rhesus Rhesus { get; set; }

        [MaxLength(50)]
        public string? Volume { get; set; }

        [MaxLength(50)]
        public string? Code { get; set; }

        public DateTime MfgDate { get; set; }

        public DateTime ExpDate { get; set; }

        public InventoryStatus Status { get; set; }

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;

        public ICollection<Crossmatch> Crossmatches { get; set; } = new List<Crossmatch>();

        public ICollection<TransactionDetail> TransactionDetails { get; set; } = new List<TransactionDetail>();


    }
}
