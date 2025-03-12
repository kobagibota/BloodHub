namespace BloodHub.Shared.DTOs
{
    public class AuthDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
}
