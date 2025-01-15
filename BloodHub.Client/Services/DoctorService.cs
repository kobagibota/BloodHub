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
    public class DoctorService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả bác sĩ.
        /// </summary>
        public async Task<ServiceResponse<List<Doctor>>> GetAllDoctors()
        {
            var endpoint = $"{DoctorEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Doctor>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin bác sĩ theo ID.
        /// </summary>
        public async Task<ServiceResponse<Doctor>> GetDoctorById(int id)
        {
            var endpoint = $"{DoctorEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Doctor>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới bác sĩ.
        /// </summary>
        public async Task<ServiceResponse<Doctor>> CreateDoctor(Doctor doctor)
        {
            var endpoint = $"{DoctorEndpoints.Create}";
            return await _httpClient.SendRequest<Doctor>(HttpMethod.Post, endpoint, doctor);
        }

        /// <summary>
        /// Cập nhật thông tin bác sĩ.
        /// </summary>
        public async Task<ServiceResponse<Doctor>> UpdateDoctor(int id, Doctor doctor)
        {
            var endpoint = $"{DoctorEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<Doctor>(HttpMethod.Put, endpoint, doctor);
        }

        /// <summary>
        /// Xóa bác sĩ theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteDoctor(int id)
        {
            var endpoint = $"{DoctorEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
