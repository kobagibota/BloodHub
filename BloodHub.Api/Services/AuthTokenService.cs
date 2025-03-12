using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Respones;
using BloodHub.Api.Extensions;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IAuthTokenService
    {
        Task LogActionAsync(int userId, Activity activity, string token);
        Task<ServiceResponse<TokenResponse>> GenerateTokensAsync(AuthDto userDto);
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<ServiceResponse<TokenResponse>> RotateRefreshTokenAsync(string token); // Xoay Refresh Token
        Task RevokeTokensByTypeAsync(int userId, TokenType tokenType); // Thu hồi tất cả token theo loại
    }

    #endregion

    public class AuthTokenService(JwtTokenService jwtTokenService, IUnitOfWork unitOfWork,
        RequestInfoProvider infoProvider) : IAuthTokenService
    {
        #region Private members

        private readonly JwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RequestInfoProvider _infoProvider = infoProvider;

        #endregion Private members

        #region Methods

        public async Task LogActionAsync(int userId, Activity activity, string token)
        {
            // Lấy thông tin IP Address và User Agent từ HttpContext
            var ipAddress = _infoProvider.GetIpAddress();
            var userAgent = _infoProvider.GetUserAgent();

            // Tạo đối tượng AuditLog
            var log = new ActivityLog
            {
                UserId = userId,
                Activity = activity,
                Token = token,
                Timestamp = DateTime.UtcNow,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _unitOfWork.ActivityLogRepository.AddAsync(log);
            await _unitOfWork.SaveAsync();
        }

        // Tạo Access Token + Refresh Token
        public async Task<ServiceResponse<TokenResponse>> GenerateTokensAsync(AuthDto userDto)
        {
            var tokenResponse = new TokenResponse
            {
                AccessToken = _jwtTokenService.GenerateAccessToken(userDto),
                RefreshToken = await GenerateRefreshTokenAsync(userDto.Id)
            };

            return new ServiceResponse<TokenResponse>
            {
                Data = tokenResponse,
                Message = "Tokens generated successfully."
            };
        }

        // Tạo và lưu Refresh Token dưới dạng hash
        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            // Tạo Refresh Token gốc (plain text)
            var refreshToken = Guid.NewGuid().ToString();

            // Băm Refresh Token trước khi lưu vào cơ sở dữ liệu
            var refreshTokenHash = BCrypt.Net.BCrypt.HashPassword(refreshToken);

            var authToken = new AuthToken
            {
                UserId = userId,
                TokenHash = refreshTokenHash, // Lưu hash của token thay vì token gốc
                TokenType = TokenType.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
            };

            // Lưu AuthToken vào cơ sở dữ liệu
            await _unitOfWork.AuthTokenRepository.AddAsync(authToken);
            await _unitOfWork.SaveAsync();

            // Ghi log việc tạo Refresh Token
            await LogActionAsync(userId, Activity.Generate, refreshToken);

            // Trả về Refresh Token gốc (plain text) cho client
            return refreshToken;
        }

        // Xoay vòng Refresh Token
        public async Task<ServiceResponse<TokenResponse>> RotateRefreshTokenAsync(string token)
        {
            var response = new ServiceResponse<TokenResponse>();
            var oldToken = await _unitOfWork.AuthTokenRepository.GetByTokenAsync(token);

            if (oldToken == null || !oldToken.IsActive)
            {
                response.Success = false;
                response.Message = "Refresh token is invalid or expired.";
                return response;
            }

            // Thu hồi Refresh Token cũ
            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.IsUsed = true;

            // Lấy thông tin người dùng để tạo Access Token
            var user = await _unitOfWork.UserRepository.GetByIdAsync(oldToken.UserId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Tìm không thấy người dùng.";
                return response;
            }

            // Lấy danh sách các vai trò của user
            var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == user.Id, r => r.Role);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            // Tạo UserDto
            var userDto = new AuthDto
            {
                Id = user.Id,
                Username = user.Username!,
                ShortName = user.ShortName,
                Roles = string.Join(",", roleNames)
            };

            // Tạo Access Token và Refresh Token mới
            var accessToken = _jwtTokenService.GenerateAccessToken(userDto);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

            // Lưu hash thay vì token thô
            var newRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);
            oldToken.ReplacedByToken = newRefreshTokenHash;
            await _unitOfWork.SaveAsync();

            response.Data = new TokenResponse { AccessToken = accessToken, RefreshToken = newRefreshToken };
            response.Message = "Token rotated successfully.";
            return response;
        }

        public async Task RevokeTokensByTypeAsync(int userId, TokenType tokenType)
        {
            await _unitOfWork.AuthTokenRepository.RevokeTokensByTypeAsync(userId, tokenType);
            await LogActionAsync(userId, Activity.Revoke, $"Revoke all {tokenType} tokens");
        }

        #endregion Methods
    }
}