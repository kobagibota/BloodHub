using BloodHub.Shared.Extentions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class ShiftDetail : BaseEntity
    {
        [Required]
        public int ShiftId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public BloodGroup BloodGroup { get; set; }

        public Rhesus Rhesus { get; set; }

        [MaxLength(50)]
        public string? Volume { get; set; }

        public int StartingQuantity { get; set; }

        public int ImportedQuantity { get; set; }

        public int ReturnedQuantity { get; set; }

        public int ExportedQuantity { get; set; }

        public int DestroyedQuantity { get; set; }

        public int EndingQuantity 
        { 
            get 
            {
                return StartingQuantity + ImportedQuantity + ReturnedQuantity - ExportedQuantity - DestroyedQuantity; 
            } 
        }

        [ForeignKey(nameof(ShiftId))]
        public virtual Shift Shift { get; set; } = null!;

        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; } = null!;
    }
}
