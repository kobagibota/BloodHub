namespace BloodHub.Shared.Request
{
    public class UserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ContactInfo { get; set; } = string.Empty;
        public List<int> RoleIds { get; set; } = new List<int>();
    }
}
