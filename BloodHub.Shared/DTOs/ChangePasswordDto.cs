namespace BloodHub.Shared.DTOs
{
    public class ChangePasswordDto
    {
        public required string CurrentPassword { get; set; } = string.Empty;

        public required string NewPassword { get; set; } = string.Empty;
    }
}
