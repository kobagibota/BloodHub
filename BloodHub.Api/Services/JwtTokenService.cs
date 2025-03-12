using BloodHub.Data.Configurations;
using BloodHub.Shared.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BloodHub.Api.Services
{
    public class JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        #region Private members

        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        #endregion Private members

        #region Methods

        /// <summary>
        /// Tạo Access Token (JWT) từ thông tin người dùng.
        /// </summary>
        /// <param name="userDto">Thông tin người dùng để tạo JWT.</param>
        /// <returns>Chuỗi Access Token đã mã hóa.</returns>
        public string GenerateAccessToken(AuthDto userDto)
        {
            // Khóa bí mật
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo Claims từ UserDto
            var userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString()), // Đổi NameIdentifier thành Sub
                new Claim(JwtRegisteredClaimNames.UniqueName, userDto.Username), // Đổi Name thành UniqueName
                new Claim("short_name", userDto.ShortName),
                new Claim("IsActive", "True")
            };

            // Thêm vai trò đúng chuẩn
            foreach (var role in userDto.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role)); // Giữ nguyên ClaimTypes.Role
            }

            // Tạo JWT
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.ValidIssuer,
                audience: _jwtSettings.ValidAudience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion Methods
    }
}