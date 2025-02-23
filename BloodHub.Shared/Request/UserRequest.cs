namespace BloodHub.Shared.Request
{
    public class UserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
