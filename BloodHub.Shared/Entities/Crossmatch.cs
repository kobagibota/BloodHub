using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class Crossmatch : BaseEntity
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int InventoryId { get; set; }

        public TestResult ResultSaline1 { get; set; }
        
        public TestResult ResultSaline2 { get; set; }

        public TestResult ResultAHG1 { get; set; }
        
        public TestResult ResultAHG2 { get; set; }

        public CrossmatchResult Result { get; set; }

        [Required]
        public int Tester { get; set; }

        public DateTime TestDate { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(InventoryId))]
        public virtual Inventory Inventory { get; set; } = null!;

        [ForeignKey(nameof(Tester))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
