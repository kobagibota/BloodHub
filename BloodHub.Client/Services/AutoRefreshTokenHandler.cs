using Blazored.LocalStorage;
using BloodHub.Client.Helpers;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace BloodHub.Client.Services
{
    public class AutoRefreshTokenHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;

        public AutoRefreshTokenHandler(IAuthService authService, ILocalStorageService localStorage, NavigationManager navigationManager)
        {
            _authService = authService;
            _localStorage = localStorage;
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Lấy AccessToken từ LocalStorage
            var accessToken = await _localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);

            // Nếu có AccessToken, thêm vào header Authorization
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            // Gửi request ban đầu
            var response = await base.SendAsync(request, cancellationToken);

            // Nếu AccessToken hết hạn, tự động làm mới
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshTokenResponse = await _authService.RefreshTokenAsync();

                if (refreshTokenResponse.Success)
                {
                    // Cập nhật token mới
                    accessToken = refreshTokenResponse.Data!.AccessToken;
                    await _localStorage.SetItemAsync(AppConstants.AccessTokenKey, accessToken);

                    // Gửi lại request với token mới
                    var newRequest = CloneRequest(request, accessToken);
                    response = await base.SendAsync(newRequest, cancellationToken);
                }
                else
                {
                    // Nếu làm mới token thất bại, điều hướng đến trang đăng nhập
                    await _localStorage.RemoveItemAsync(AppConstants.AccessTokenKey);
                    await _localStorage.RemoveItemAsync(AppConstants.RefreshTokenKey);
                    _navigationManager.NavigateTo("/login", true);
                }
            }

            return response;
        }

        /// <summary>
        /// Tạo bản sao của HttpRequestMessage với AccessToken mới.
        /// </summary>
        private HttpRequestMessage CloneRequest(HttpRequestMessage request, string accessToken)
        {
            var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);

            // Sao chép header
            foreach (var header in request.Headers)
            {
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Sao chép nội dung nếu có
            if (request.Content != null)
            {
                newRequest.Content = new StreamContent(request.Content.ReadAsStream());
                foreach (var header in request.Content.Headers)
                {
                    newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // Thêm AccessToken mới vào header Authorization
            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return newRequest;
        }
    }
}
