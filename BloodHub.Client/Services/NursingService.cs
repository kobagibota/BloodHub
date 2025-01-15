using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class NursingService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả điều dưỡng.
        /// </summary>
        public async Task<ServiceResponse<List<Nursing>>> GetAllNursings()
        {
            var endpoint = $"{NursingEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Nursing>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin điều dưỡng theo ID.
        /// </summary>
        public async Task<ServiceResponse<Nursing>> GetNursingById(int id)
        {
            var endpoint = $"{NursingEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Nursing>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới điều dưỡng.
        /// </summary>
        public async Task<ServiceResponse<Nursing>> CreateNursing(Nursing nursing)
        {
            var endpoint = $"{NursingEndpoints.Create}";
            return await _httpClient.SendRequest<Nursing>(HttpMethod.Post, endpoint, nursing);
        }

        /// <summary>
        /// Cập nhật thông tin điều dưỡng.
        /// </summary>
        public async Task<ServiceResponse<Nursing>> UpdateNursing(int id, Nursing nursing)
        {
            var endpoint = $"{NursingEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<Nursing>(HttpMethod.Put, endpoint, nursing);
        }

        /// <summary>
        /// Xóa điều dưỡng theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteNursing(int id)
        {
            var endpoint = $"{NursingEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
