using Blazored.LocalStorage;
using BloodHub.Client.Helpers;
using System.Net.Http.Headers;

namespace BloodHub.Client.Services
{
    public class AutoRefreshTokenHandler : DelegatingHandler
    {
        private readonly IAuthService _authService;
        private readonly ILocalStorageService _localStorage;

        public AutoRefreshTokenHandler(IAuthService authService, ILocalStorageService localStorage)
        {
            _authService = authService;
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Lấy AccessToken từ LocalStorage
            var accessToken = await _localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await base.SendAsync(request, cancellationToken);

            // Nếu AccessToken hết hạn, tự động làm mới
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshTokenResponse = await _authService.RefreshTokenAsync();

                if (refreshTokenResponse.Success)
                {
                    // Cập nhật token mới
                    accessToken = refreshTokenResponse.Data!.AccessToken;

                    // Gửi lại request với token mới
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Tạo lại request để tránh lỗi "Cannot send the same request message multiple times"
                    var newRequest = new HttpRequestMessage(request.Method, request.RequestUri); 
                    foreach (var header in request.Headers) 
                        newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value); 
                    
                    if (request.Content != null) 
                    { 
                        newRequest.Content = new StreamContent(await request.Content.ReadAsStreamAsync());
                        foreach (var header in request.Content.Headers) 
                            newRequest.Content.Headers.TryAddWithoutValidation(header.Key, header.Value); 
                    }

                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            return response;
        }
    }
}
