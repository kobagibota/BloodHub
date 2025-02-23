using Microsoft.AspNetCore.Identity;
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
        RequestInfoProvider infoProvider, UserManager<User> userManager) : IAuthTokenService
    {
        #region Private members

        private readonly JwtTokenService _jwtTokenService = jwtTokenService;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RequestInfoProvider _infoProvider = infoProvider;
        private readonly UserManager<User> _userManager = userManager;

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

        public async Task<ServiceResponse<TokenResponse>> GenerateTokensAsync(AuthDto userDto)
        {
            var tokenResponse = new TokenResponse();

            var accessToken = _jwtTokenService.GenerateAccessToken(userDto);
            var refreshTokenResponse = await GenerateRefreshTokenAsync(userDto.Id);

            tokenResponse.AccessToken = accessToken;
            tokenResponse.RefreshToken = refreshTokenResponse;

            return new ServiceResponse<TokenResponse>
            {
                Data = tokenResponse,
                Message = "Tokens generated successfully."
            };
        }

        public async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var refreshToken = new AuthToken
            {
                UserId = userId,
                Token = Guid.NewGuid().ToString(),
                TokenType = TokenType.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };

            await _unitOfWork.AuthTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.SaveAsync();
            await LogActionAsync(userId, Activity.Generate, refreshToken.Token);

            return refreshToken.Token;
        }

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
            var roles = await _userManager.GetRolesAsync(user);

            // Tạo UserDto
            var userDto = new AuthDto
            {
                Id = user.Id,
                Username = user.UserName!,
                FullName = user.FullName,
                Roles = string.Join(",", roles)
            };

            // Tạo Access Token và Refresh Token mới
            var accessToken = _jwtTokenService.GenerateAccessToken(userDto);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.Id);

            // Cập nhật Refresh Token cũ
            oldToken.ReplacedByToken = newRefreshToken;
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