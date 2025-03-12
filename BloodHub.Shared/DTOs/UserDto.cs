namespace BloodHub.Shared.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;

        public string? ContactInfo { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public bool IsOnDuty { get; set; }

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
