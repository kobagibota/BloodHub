using BloodHub.Client.Helpers;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Respones;
using static BloodHub.Client.Helpers.AppConstants;

namespace BloodHub.Client.Services
{
    /// <summary>
    /// Khởi tạo dịch vụ với HttpClient và LocalStorage.
    /// </summary>
    public class ProductService(HttpClientHelper httpClient)
    {
        #region Private members

        private readonly HttpClientHelper _httpClient = httpClient;

        #endregion

        #region Methods

        /// <summary>
        /// Lấy danh sách tất cả sản phẩm.
        /// </summary>
        public async Task<ServiceResponse<List<Product>>> GetAllProducts()
        {
            var endpoint = $"{ProductEndpoints.GetAll}";
            return await _httpClient.SendRequest<List<Product>>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo ID.
        /// </summary>
        public async Task<ServiceResponse<Product>> GetProductById(int id)
        {
            var endpoint = $"{ProductEndpoints.GetById}/{id}";
            return await _httpClient.SendRequest<Product>(HttpMethod.Get, endpoint);
        }

        /// <summary>
        /// Tạo mới sản phẩm.
        /// </summary>
        public async Task<ServiceResponse<Product>> CreateProduct(Product product)
        {
            var endpoint = $"{ProductEndpoints.Create}";
            return await _httpClient.SendRequest<Product>(HttpMethod.Post, endpoint, product);
        }

        /// <summary>
        /// Cập nhật thông tin sản phẩm.
        /// </summary>
        public async Task<ServiceResponse<Product>> UpdateProduct(int id, Product product)
        {
            var endpoint = $"{ProductEndpoints.Update}/{id}";
            return await _httpClient.SendRequest<Product>(HttpMethod.Put, endpoint, product);
        }

        /// <summary>
        /// Xóa sản phẩm theo ID.
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteProduct(int id)
        {
            var endpoint = $"{ProductEndpoints.Delete}/{id}";
            return await _httpClient.SendRequest<bool>(HttpMethod.Delete, endpoint);
        }

        #endregion
    }
}
