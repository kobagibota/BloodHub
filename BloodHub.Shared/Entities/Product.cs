using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Product : BaseEntity
    {
        [Required, MaxLength(250)]
        public string ProductName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Unit { get; set; } = string.Empty;

        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        public virtual ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();

    }
}
