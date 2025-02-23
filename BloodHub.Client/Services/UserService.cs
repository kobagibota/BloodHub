using BloodHub.Client.Helpers;
using BloodHub.Shared.DTOs;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class UserService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả người dùng.
        /// </summary>
        public async Task<ServiceResponse<List<UserDto>>> GetAllUsers()
        {
            var endpoint = $"{UserEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<UserDto>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo ID.
        /// </summary>
        public async Task<ServiceResponse<UserDto>> GetUserById(int id)
        {
            var endpoint = $"{UserEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<UserDto>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới người dùng.
        /// </summary>
        public async Task<ServiceResponse<UserDto>> CreateUser(UserRequest request)
        {
            var endpoint = $"{UserEndpoints.Create}";
            return await _httpClient.SendRequest<UserDto>(HttpMethod.Post, endpoint, request);
        }

        /// <summary>
        /// Cập nhật thông tin người dùng.
        /// </summary>
        public async Task<ServiceResponse<UserDto>> UpdateUser(int id, UserRequest request)
        {
            var endpoint = $"{UserEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<UserDto>(HttpMethod.Put, endpoint, request);
        }

        /// <summary>
        /// Cập nhật trạng thái người dùng.
        /// </summary>
        public async Task<ServiceResponse<bool>> ToggleActive(int id)
        {
            var endpoint = $"{UserEndpoints.ToggleActive}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Patch, endpoint, id);
        }

        /// <summary>
        /// Xóa người dùng theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteUser(int id)
        {
            var endpoint = $"{UserEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        /// <summary>
        /// Lấy danh sách người trực.
        /// </summary>
        public async Task<ServiceResponse<List<ShiftUserDto>>> GetAvailableUsersForShifts()
        {
            var endpoint = $"{UserEndpoints.GetAvailableUsersForShifts}";
            return await _httpClient.SendRequest<List<ShiftUserDto>>(HttpMethod.Get, endpoint);
        }

        #endregion
    }
}
