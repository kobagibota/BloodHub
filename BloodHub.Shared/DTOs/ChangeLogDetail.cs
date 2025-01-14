namespace BloodHub.Shared.DTOs
{
    public class ChangeLogDetail
    {
        public string ColumnName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }

}
