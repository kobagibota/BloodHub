using Blazored.LocalStorage;
using BloodHub.Shared.Respones;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BloodHub.Client.Helpers
{
    public class HttpClientHelper(NavigationManager navigationManager, HttpClient httpClient, ILocalStorageService localStorage)
    {
        #region Private members

        private readonly HttpClient _httpClient = httpClient;
        private readonly ILocalStorageService _localStorage = localStorage;
        private readonly NavigationManager _navigationManager = navigationManager;

        #endregion

        /// <summary>
        /// Gửi request HTTP với khả năng tự động thêm token và xử lý phản hồi.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của phản hồi.</typeparam>
        /// <param name="httpClient">HttpClient được cung cấp từ IHttpClientFactory.</param>
        /// <param name="localStorage">Dịch vụ lưu trữ local để lấy token.</param>
        /// <param name="method">Phương thức HTTP (GET, POST, PUT, DELETE).</param>
        /// <param name="url">URL API cần gọi.</param>
        /// <param name="content">Nội dung request (nếu có).</param>
        /// <returns>Đối tượng ServiceResponse chứa dữ liệu hoặc lỗi.</returns>
        public async Task<ServiceResponse<T>> SendRequest<T>(HttpMethod method, string url, object? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            // Nếu có nội dung, thêm vào request
            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }

            // Lấy token từ LocalStorage
            var accessToken = await _localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            { // Remove Authorization header if token is not available
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }

            // Gửi request và nhận phản hồi
            var response = await _httpClient.SendAsync(request);

            var result = new ServiceResponse<T>();

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Conflict:
                    result = await DeserializeResponse<T>(response);
                    break;
                case HttpStatusCode.NoContent:
                    result.Success = true;
                    result.Message = "No content.";
                    break;
                case HttpStatusCode.Unauthorized:
                    result.Success = false;
                    result.Message = "Bạn vui lòng đăng nhập để tiếp tục.";
                    // Điều hướng đến trang đăng nhập nếu Unauthorized
                    _navigationManager.NavigateTo("/login", true);
                    break;
                case HttpStatusCode.Forbidden:
                    result.Success = false;
                    result.Message = "Bạn không có quyền truy cập chức năng này.";
                    break;
                case HttpStatusCode.NotFound:
                    result.Success = false;
                    result.Message = "Resource not found.";
                    break;
                case HttpStatusCode.InternalServerError:
                    result.Success = false;
                    result.Message = "Lỗi kết nối đến máy chủ.";
                    break;
                default:
                    result.Success = false;
                    result.Message = $"Unexpected status code: {response.StatusCode}.";
                    break;
            }

            return result;
        }

        private static async Task<ServiceResponse<T>> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var contentResult = await response.Content.ReadFromJsonAsync<ServiceResponse<T>>();
            return contentResult ?? new ServiceResponse<T>
            {
                Success = false,
                Message = "Failed to deserialize response."
            };
        }
    }
}
