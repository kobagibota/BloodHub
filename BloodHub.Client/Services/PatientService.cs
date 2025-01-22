using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class PatientService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả bệnh nhân.
        /// </summary>
        public async Task<ServiceResponse<List<Patient>>> GetAllPatients()
        {
            var endpoint = $"{PatientEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Patient>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin bệnh nhân theo ID.
        /// </summary>
        public async Task<ServiceResponse<Patient>> GetPatientById(int id)
        {
            var endpoint = $"{PatientEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Patient>(HttpMethod.Get, endpoint);
        }
              
        /// <summary>
        /// Tạo mới bệnh nhân.
        /// </summary>
        public async Task<ServiceResponse<Patient>> CreatePatient(Patient patient)
        {
            var endpoint = $"{PatientEndpoints.Create}";
            return await _httpClient.SendRequest<Patient>(HttpMethod.Post, endpoint, patient);
        }

        /// <summary>
        /// Cập nhật thông tin bệnh nhân.
        /// </summary>
        public async Task<ServiceResponse<Patient>> UpdatePatient(int id, Patient patient)
        {
            var endpoint = $"{PatientEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<Patient>(HttpMethod.Put, endpoint, patient);
        }

        /// <summary>
        /// Xóa bệnh nhân theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeletePatient(int id)
        {
            var endpoint = $"{PatientEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
