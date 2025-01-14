using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodHub.Shared.Entities
{
    public class ShiftUser
    {
        [Key]
        public int UserId { get; set; }

        [Key]
        public int ShiftId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(ShiftId))]
        public virtual Shift Shift { get; set; } = null!;
    }
}
