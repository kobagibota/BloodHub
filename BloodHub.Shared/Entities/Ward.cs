using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Ward : BaseEntity
    {
        [Required, MaxLength(250)]
        public required string WardName { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
