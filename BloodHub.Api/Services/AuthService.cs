using BloodHub.Shared.DTOs;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IAuthService
    {
        Task<ServiceResponse<TokenResponse>> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<bool>> LogoutAsync(int userId, string accessToken);
        Task<ServiceResponse<bool>> ChangePassword(int userId, ChangePasswordDto request);
        Task<ServiceResponse<TokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<ServiceResponse<bool>> RevokeAllTokensAsync(int userId);
    }

    #endregion Interface

    public class AuthService(IUnitOfWork unitOfWork, IAuthTokenService authTokenService) : IAuthService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthTokenService _authTokenService = authTokenService;

        #endregion Private properties

        #region Methods

        public async Task<ServiceResponse<TokenResponse>> LoginAsync(LoginDto loginDto)
        {
            var response = new ServiceResponse<TokenResponse>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Username == loginDto.UserName);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Tài khoản không tồn tại.";
                    return response;
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Mật khẩu không chính xác.";
                    return response;
                }

                var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == user.Id, r => r.Role);
                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                var userDto = user.ConvertToAuthDto(roleNames);

                // Tạo Access Token và Refresh Token
                var tokenResponse = await _authTokenService.GenerateTokensAsync(userDto);

                response.Success = true;
                response.Data = tokenResponse.Data;
                response.Message = "Đăng nhập thành công!";

                // Audit log đăng nhập thành công
                await _authTokenService.LogActionAsync(user.Id, Activity.Login, response.Data!.AccessToken);
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình đăng nhập.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ChangePassword(int userId, ChangePasswordDto request)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Tài khoản không tồn tại.";
                    return response;
                }

                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    response.Success = false;
                    response.Message = "Mật khẩu không chính xác.";
                    return response;
                }

                // Băm mật khẩu mới và cập nhật
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);
                // workFactor: 12 là số vòng băm (độ mạnh của hash), giá trị mặc định là 10, nhưng 12 là mức khuyến nghị

                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveAsync();
                
                response.Data = true;
                response.Success = true;
                response.Message = "Đổi mật khẩu thành công.";

                // Log đổi mật khẩu
                await _authTokenService.LogActionAsync(userId, Activity.ChangePassword, "Password updated");                
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình đổi mật khẩu";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> LogoutAsync(int userId, string accessToken)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                // Thu hồi tất cả Refresh Tokens
                await _authTokenService.RevokeTokensByTypeAsync(userId, TokenType.RefreshToken);

                // Log action logout
                await _authTokenService.LogActionAsync(userId, Activity.Logout, accessToken);

                response.Success = true;
                response.Data = true;
                response.Message = "Đăng xuất thành công.";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra khi đăng xuất";
            }
            return response;
        }

        public async Task<ServiceResponse<TokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            var response = await _authTokenService.RotateRefreshTokenAsync(refreshToken);
            if (!response.Success)
            {
                // Log nếu Refresh Token không hợp lệ
                var token = await _unitOfWork.AuthTokenRepository.GetByTokenAsync(refreshToken);
                if (token != null)
                {
                    await _authTokenService.LogActionAsync(token.UserId, Activity.InvalidToken, refreshToken);
                }
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> RevokeAllTokensAsync(int userId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                // Thu hồi tất cả các token
                await _authTokenService.RevokeTokensByTypeAsync(userId, TokenType.AccessToken);
                await _authTokenService.RevokeTokensByTypeAsync(userId, TokenType.RefreshToken);

                // Log revoke token
                await _authTokenService.LogActionAsync(userId, Activity.Revoke, "Tất cả các token đã bị thu hồi.");

                response.Success = true;
                response.Data = true;
                response.Message = "Đã thu hồi tất cả token của người dùng.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình thu hồi token: {ex.Message}";
            }

            return response;
        }

        #endregion Methods
    }
}