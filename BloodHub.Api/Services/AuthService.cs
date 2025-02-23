using Microsoft.AspNetCore.Identity;
using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
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

    public class AuthService(IUnitOfWork unitOfWork, IAuthTokenService authTokenService, 
        UserManager<User> userManager, SignInManager<User> signInManager) : IAuthService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IAuthTokenService _authTokenService = authTokenService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        #endregion Private properties


        //RegisterAsync(UserRegisterRequest request) – Đăng ký user mới.
        //LoginAsync(LoginRequest request) – Đăng nhập & tạo JWT Token.
        //RefreshTokenAsync(string refreshToken) – Cấp lại access token.
        //ChangePasswordAsync(ChangePasswordRequest request) – Đổi mật khẩu.
        //AssignRolesAsync(int userId, List<string> roles) – Gán vai trò cho user.

        #region Methods

        public async Task<ServiceResponse<TokenResponse>> LoginAsync(LoginDto loginDto)
        {
            var response = new ServiceResponse<TokenResponse>();

            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Tài khoản không tồn tại.";
                    return response;
                }

                if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    response.Success = false;
                    response.Message = "Mật khẩu không chính xác.";
                    return response;
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                var roles = await _userManager.GetRolesAsync(user);
                var userDto = user.ConvertToAuthDto(roles);

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
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Tài khoản này không tồn tại. \n Mời bạn xem lại.";
                    return response;
                }

                var passwordHasher = new PasswordHasher<User>();
                var veryfyResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.CurrentPassword); 
                if (veryfyResult != PasswordVerificationResult.Success) 
                { 
                    response.Success = false; 
                    response.Message = "Mật khẩu hiện tại không chính xác.";

                    return response; 
                }
                user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    response.Data = true;
                    response.Success = true;
                    response.Message = "Đổi mật khẩu thành công.";

                    // Log đổi mật khẩu
                    await _authTokenService.LogActionAsync(userId, Activity.ChangePassword, "Password updated");
                }
                else
                {
                    response.Success = false;
                    response.Message = "Đổi mật khẩu không thành công.";
                }
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

                await _signInManager.SignOutAsync();

                // Log action logout
                await _authTokenService.LogActionAsync(userId, Activity.Logout, accessToken);

                response.Success = true;
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
                await _authTokenService.LogActionAsync(userId, Activity.Revoke, "Revoke all tokens");

                response.Success = true;
                response.Message = "Đã thu hồi tất cả token của người dùng.";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Có lỗi xảy ra";
            }

            return response;
        }

        #endregion Methods
    }
}