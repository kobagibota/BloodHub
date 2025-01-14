using Blazored.LocalStorage;
using BloodHub.Client.Helpers;
using BloodHub.Shared.DTOs;
using BloodHub.Shared.Respones;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace BloodHub.Client.Services
{
    #region Interface  

    public interface IAuthService
    {
        Task<ServiceResponse<bool>> LoginAsync(LoginDto loginDto); // Đăng nhập  
        Task<ServiceResponse<TokenResponse>> RefreshTokenAsync(); // Làm mới Token  
        Task LogoutAsync(); // Đăng xuất  
        Task<bool> IsLoggedInAsync(); // Kiểm tra trạng thái đăng nhập  
        Task<string> GetCurrentUserAsync(); // Lấy thông tin người dùng hiện tại  
        Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto); // Đổi mật khẩu  
    }

    #endregion

    public class AuthService(HttpClient httpClient, ILocalStorageService localStorage,
                             AuthenticationStateProvider authStateProvider) : IAuthService
    {
        #region Private Members  

        private readonly HttpClient _httpClient = httpClient;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly CustomAuthenticationStateProvider _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;

        #endregion
        
        #region Methods  

        public async Task<ServiceResponse<bool>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await HttpClientHelper.SendRequest<TokenResponse>(
                    _httpClient, _localStorage, HttpMethod.Post, AppConstants.ApiEndpoints.Login, loginDto);

                if (response == null || !response.Success)
                {
                    return new ServiceResponse<bool>
                    {
                        Data = false,
                        Success = false,
                        Message = response?.Message ?? "Lỗi không xác định."
                    };
                }

                await StoreTokensAsync(response.Data!.AccessToken, response.Data.RefreshToken);
                _authStateProvider.NotifyUserAuthentication(response.Data.AccessToken); // Gọi phương thức ngay tại đây  

                return new ServiceResponse<bool>
                {
                    Data = true,
                    Success = true
                };
            }
            catch (HttpRequestException ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = ex.Message };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool> { Success = false, Message = $"Đã xảy ra lỗi: {ex.Message}" };
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);
            return !string.IsNullOrEmpty(token);
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync(AppConstants.AccessTokenKey);
            await _localStorage.RemoveItemAsync(AppConstants.RefreshTokenKey);
            _authStateProvider.NotifyUserLogout(); // Gọi phương thức tại đây  
        }

        public async Task<ServiceResponse<TokenResponse>> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = await _localStorage.GetItemAsync<string>(AppConstants.RefreshTokenKey);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return new ServiceResponse<TokenResponse>
                    {
                        Success = false,
                        Message = "Không tìm thấy Refresh Token. Vui lòng đăng nhập lại."
                    };
                }

                var response = await HttpClientHelper.SendRequest<TokenResponse>(
                    _httpClient, _localStorage, HttpMethod.Post, AppConstants.ApiEndpoints.RefreshToken, new { RefreshToken = refreshToken });

                if (response == null || !response.Success)
                {
                    return new ServiceResponse<TokenResponse>
                    {
                        Data = new TokenResponse(),
                        Success = false,
                        Message = response?.Message ?? "Làm mới token thất bại. Vui lòng đăng nhập lại."
                    };
                }

                await StoreTokensAsync(response.Data!.AccessToken, response.Data.RefreshToken);
                _authStateProvider.NotifyUserAuthentication(response.Data.AccessToken); // Gọi tại đây  

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<TokenResponse>
                {
                    Success = false,
                    Message = $"Đã xảy ra lỗi: {ex.Message}"
                };
            }
        }

        public async Task<string> GetCurrentUserAsync()
        {
            string? accessToken = await _localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);
            if (string.IsNullOrEmpty(accessToken)) return "Khách do null";

            var jwtToken = new JwtSecurityTokenHandler().ReadToken(accessToken) as JwtSecurityToken;
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == "full_name")?.Value ?? "Khách";
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            return await HttpClientHelper.SendRequest<bool>(
                _httpClient, _localStorage, HttpMethod.Post, AppConstants.ApiEndpoints.ChangePassword, changePasswordDto);
        }

        private async Task StoreTokensAsync(string accessToken, string refreshToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                await _localStorage.SetItemAsync(AppConstants.AccessTokenKey, accessToken);
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _localStorage.SetItemAsync(AppConstants.RefreshTokenKey, refreshToken);
            }
        }

        #endregion
    }
}