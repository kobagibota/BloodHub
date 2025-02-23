using BloodHub.Shared.Extentions;

namespace BloodHub.Shared.DTOs
{
    public class ShiftDetailDto
    {
        public int Id { get; set; }

        public int ShiftId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public BloodGroup BloodGroup { get; set; }

        public Rhesus Rhesus { get; set; }

        public string? Volume { get; set; }

        public int StartingQuantity { get; set; }

        public int ImportedQuantity { get; set; }

        public int ReturnedQuantity { get; set; }

        public int ExportedQuantity { get; set; }

        public int DestroyedQuantity { get; set; }

        public int EndingQuantity { get; set; }
    }
}
