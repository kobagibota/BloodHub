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
    public class OrderService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả chỉ định.
        /// </summary>
        public async Task<ServiceResponse<List<Order>>> GetAllOrders()
        {
            var endpoint = $"{OrderEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Order>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin chỉ định theo ID.
        /// </summary>
        public async Task<ServiceResponse<Order>> GetOrderById(int id)
        {
            var endpoint = $"{OrderEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Order>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin chỉ định theo ID bệnh nhân.
        /// </summary>
        public async Task<ServiceResponse<List<OrderDto>>> GetOrderByPatientId(int id)
        {
            var endpoint = $"{OrderEndpoints.GetByPatientId}/{id}";
            return await _httpClient.SendRequest<List<OrderDto>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới chỉ định.
        /// </summary>
        public async Task<ServiceResponse<OrderDto>> CreateOrder(OrderRequest order)
        {
            var endpoint = $"{OrderEndpoints.Create}";
            return await _httpClient.SendRequest<OrderDto>(HttpMethod.Post, endpoint, order);
        }

        /// <summary>
        /// Cập nhật thông tin chỉ định.
        /// </summary>
        public async Task<ServiceResponse<OrderDto>> UpdateOrder(int id, OrderRequest order)
        {
            var endpoint = $"{OrderEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<OrderDto>(HttpMethod.Put, endpoint, order);
        }

        /// <summary>
        /// Xóa chỉ định theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteOrder(int id)
        {
            var endpoint = $"{OrderEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
