using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Ward : BaseEntity
    {
        [Required, MaxLength(250)]
        public string WardName { get; set; } = string.Empty;

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
