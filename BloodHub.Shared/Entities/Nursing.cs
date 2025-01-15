using System.ComponentModel.DataAnnotations;

namespace BloodHub.Shared.Entities
{
    public class Nursing : BaseEntity
    {
        [Required, MaxLength(200)]
        public string NursingName { get; set; } = string.Empty;

        public bool IsHide { get; set; } = false;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    }
}
