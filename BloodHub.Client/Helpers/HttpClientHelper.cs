using Blazored.LocalStorage;
using BloodHub.Shared.Respones;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BloodHub.Client.Helpers
{
    public static class HttpClientHelper
    {
        public static async Task<ServiceResponse<T>> SendRequest<T>(
            HttpClient httpClient, ILocalStorageService localStorage, HttpMethod method, string url, object? content = null)
        {
            var request = new HttpRequestMessage(method, url);

            if (content != null)
            {
                request.Content = JsonContent.Create(content);
            }

            var accessToken = await localStorage.GetItemAsync<string>(AppConstants.AccessTokenKey);
            if (!string.IsNullOrEmpty(accessToken)) 
            { 
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); 
            }
            else
            { // Remove Authorization header if token is not available
              httpClient.DefaultRequestHeaders.Authorization = null; 
            }

            var response = await httpClient.SendAsync(request);
            var result = new ServiceResponse<T>();

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                result.Success = false;
                result.Message = "Bạn không có quyền truy cập chức năng này."; 
                
                return result;
            }

            var contentResult = await response.Content.ReadFromJsonAsync<ServiceResponse<T>>();
            if (contentResult != null)
            {
                return contentResult;
            }

            result.Success = false;
            result.Message = $"Failed to deserialize response from {url}";

            return result;
        }
    }
}
