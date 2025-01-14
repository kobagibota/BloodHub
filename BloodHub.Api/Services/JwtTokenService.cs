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
        public string GenerateAccessToken(UserDto userDto)
        {
            // Khóa bí mật
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Tạo Claims từ UserDto
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                new Claim(ClaimTypes.Name, userDto.Username),
                new Claim("full_name", userDto.FullName),
                new Claim("IsActive", "True") // Add the IsActive claim
            };

            // Thêm các vai trò vào Claims
            var roles = userDto.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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