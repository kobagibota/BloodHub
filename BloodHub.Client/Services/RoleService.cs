using Blazored.LocalStorage;
using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;
using System.Net;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class RoleService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả vai trò.
        /// </summary>
        public async Task<ServiceResponse<IEnumerable<string>>> GetAllRoles()
        {
            var endpoint = $"{RoleEndpoints.GetAll}";
            return await _httpClient.SendRequest<IEnumerable<string>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới vai trò.
        /// </summary>
        public async Task<ServiceResponse<Role>> CreateRole(Role role)
        {
            var endpoint = $"{RoleEndpoints.Create}";
            return await _httpClient.SendRequest<Role>(HttpMethod.Post, endpoint, role);
        }

        /// <summary>
        /// Xóa vai trò theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteRole(int id)
        {
            var endpoint = $"{RoleEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
