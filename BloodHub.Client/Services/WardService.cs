using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class WardService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả khoa phòng.
        /// </summary>
        public async Task<ServiceResponse<List<Ward>>> GetAllWards()
        {
            var endpoint = $"{WardEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Ward>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin khoa phòng theo ID.
        /// </summary>
        public async Task<ServiceResponse<Ward>> GetWardById(int id)
        {
            var endpoint = $"{WardEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Ward>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới khoa phòng.
        /// </summary>
        public async Task<ServiceResponse<Ward>> CreateWard(Ward ward)
        {
            var endpoint = $"{WardEndpoints.Create}";
            return await _httpClient.SendRequest<Ward>(HttpMethod.Post, endpoint, ward);
        }

        /// <summary>
        /// Cập nhật thông tin khoa phòng.
        /// </summary>
        public async Task<ServiceResponse<Ward>> UpdateWard(int id, Ward ward)
        {
            var endpoint = $"{WardEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<Ward>(HttpMethod.Put, endpoint, ward);
        }

        /// <summary>
        /// Xóa khoa phòng theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteWard(int id)
        {
            var endpoint = $"{WardEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
