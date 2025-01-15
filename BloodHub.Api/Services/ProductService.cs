using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IProductService
    {
        Task<ServiceResponse<IEnumerable<Product>>> GetAll();
        Task<ServiceResponse<Product?>> GetById(int productId);
        Task<ServiceResponse<Product>> Add(ProductRequest request);
        Task<ServiceResponse<Product?>> Update(int productId, ProductRequest request);
        Task<ServiceResponse<bool>> Delete(int productId);
    }

    #endregion

    public class ProductService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IProductService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Product>> Add(ProductRequest request)
        {
            var response = new ServiceResponse<Product>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                var duplicate = await _unitOfWork.ProductRepository.IsExists(0, request);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên sản phẩm ''{request.ProductName}'' với đơn vị tính là ''{request.Unit}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm product
                var newProduct = new Product()
                {
                    ProductName = request.ProductName,
                    Unit = request.Unit
                };
                await _unitOfWork.ProductRepository.AddAsync(newProduct);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newProduct;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int productId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldProduct = new Product
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Unit = product.Unit
                };

                // Xoá sản phẩm
                _unitOfWork.ProductRepository.Remove(product);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldProduct, null, nameof(Product), productId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa sản phẩm: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Product>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Product>>();

            try
            {
                var products = await _unitOfWork.ProductRepository.GetAllAsync();

                response.Success = true;
                response.Data = products;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách sản phẩm.";
            }

            return response;
        }

        public async Task<ServiceResponse<Product?>> GetById(int productId)
        {
            var response = new ServiceResponse<Product?>();

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy sản phẩm.";
                return response;
            }

            response.Success = true;
            response.Data = product;
            return response;
        }

        public async Task<ServiceResponse<Product?>> Update(int productId, ProductRequest request)
        {
            var response = new ServiceResponse<Product?>();

            try
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy sản phẩm.";
                    return response;
                }

                bool duplicate = await _unitOfWork.ProductRepository.IsExists(productId, request);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên sản phẩm ''{request.ProductName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldProduct = new Product
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Unit = request.Unit
                };

                // Cập nhật thông tin sản phẩm
                product.ProductName = request.ProductName ?? product.ProductName;
                product.Unit = request.Unit ?? product.Unit;

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldProduct, product, nameof(Product), productId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = product;
                response.Success = true;
                response.Message = "Cập nhật thông tin sản phẩm thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật sản phẩm: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
