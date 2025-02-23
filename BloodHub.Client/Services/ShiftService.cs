using BloodHub.Client.Helpers;
using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class ShiftService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả ca trực.
        /// </summary>
        public async Task<ServiceResponse<List<ShiftDto>>> GetAllShifts()
        {
            var endpoint = $"{ShiftEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<ShiftDto>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin ca trực theo ID.
        /// </summary>
        public async Task<ServiceResponse<ShiftDto>> GetShiftById(int id)
        {
            var endpoint = $"{ShiftEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<ShiftDto>(HttpMethod.Get, endpoint);
        }
              
        /// <summary>
        /// Tạo mới ca trực.
        /// </summary>
        public async Task<ServiceResponse<ShiftDto>> CreateShift(ShiftRequest request)
        {
            var endpoint = $"{ShiftEndpoints.Create}";
            return await _httpClient.SendRequest<ShiftDto>(HttpMethod.Post, endpoint, request);
        }

        /// <summary>
        /// Cập nhật thông tin ca trực.
        /// </summary>
        public async Task<ServiceResponse<ShiftDto>> UpdateShift(int id, ShiftRequest request)
        {
            var endpoint = $"{ShiftEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<ShiftDto>(HttpMethod.Put, endpoint, request);
        }

        /// <summary>
        /// Giao ca trực cho người khác.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> Handover(int id, ShiftHandoverRequest request)
        {
            var endpoint = $"{ShiftEndpoints.ShiftHandover}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Put, endpoint, request);
        }

        /// <summary>
        /// Xác nhận việc giao ca trực.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ServiceResponse<bool>> ConfirmHandover(int id, ShiftConfirmHandoverRequest request)
        {
            var endpoint = $"{ShiftEndpoints.ShiftConfirmHandover}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Put, endpoint, request);
        }

        /// <summary>
        /// Xóa ca trực theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteShift(int id)
        {
            var endpoint = $"{ShiftEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
