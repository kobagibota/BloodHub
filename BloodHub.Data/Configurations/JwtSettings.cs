namespace BloodHub.Data.Configurations
{
    public class JwtSettings
    {
        public string SecurityKey { get; set; } = string.Empty; // Secret key để ký JWT
        public string ValidIssuer { get; set; } = string.Empty; // Tên Issuer
        public string ValidAudience { get; set; } = string.Empty; // Tên Audience
        public int ExpiresInMinutes { get; set; } = 60; // Thời gian hết hạn của Access Token
        public int RefreshTokenExpiryDays { get; set; } = 7; // Thời gian hết hạn của Refresh Token
    }
}