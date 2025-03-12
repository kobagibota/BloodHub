namespace BloodHub.Shared.Request
{
    public class UserRequest
    {
        public required string Username { get; set; }

        public required string Title { get; set; }          // chức danh

        public required string FirstName { get; set; }      // Tên đệm

        public required string LastName { get; set; }       // Họ

        public string? ContactInfo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsOnDuty { get; set; } = true;

        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
