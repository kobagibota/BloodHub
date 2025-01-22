using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IOrderService
    {
        Task<ServiceResponse<IEnumerable<Order>>> GetAll();
        Task<ServiceResponse<Order?>> GetById(int orderId);
        Task<ServiceResponse<IEnumerable<OrderDto>>> GetByPatientId(int patientId);
        Task<ServiceResponse<OrderDto>> Add(OrderRequest request);
        Task<ServiceResponse<OrderDto>> Update(int orderId, OrderRequest request);
        Task<ServiceResponse<bool>> Delete(int orderId);
    }

    #endregion

    public class OrderService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IOrderService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<OrderDto>> Add(OrderRequest request)
        {
            var response = new ServiceResponse<OrderDto>();

            try
            {
                // Kiểm tra đầu vào
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";
                    return response;
                }

                // Tạo đối tượng Order mới từ request
                var newOrder = new Order
                {
                    PatientId = request.PatientId,
                    WardId = request.WardId,
                    DoctorId = request.DoctorId,
                    OrderDate = request.OrderDate,
                    Diagnosis = request.Diagnosis,
                    Room = request.Room
                };

                // Thêm Order vào cơ sở dữ liệu
                await _unitOfWork.OrderRepository.AddAsync(newOrder);
                await _unitOfWork.SaveAsync();

                // Danh sách lỗi để gom các thông báo
                var errorMessages = new List<string>();

                var ward = await _unitOfWork.WardRepository.GetByIdAsync(newOrder.WardId);
                if (ward == null)
                {
                    errorMessages.Add("Khoa phòng không tồn tại.");
                }
                
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(newOrder.DoctorId);
                if (doctor == null)
                {
                    errorMessages.Add("Bác sĩ không tồn tại.");
                }
               
                // Nếu có lỗi, trả về thông báo lỗi
                if (errorMessages.Any())
                {
                    response.Success = false;
                    response.Message = string.Join(", ", errorMessages); // Gộp tất cả thông báo lỗi
                    return response;
                }

                var orderResponse = new OrderDto()
                {
                    Id = newOrder.Id,
                    PatientId = newOrder.PatientId,
                    DoctorId = newOrder.DoctorId,
                    DoctorName = doctor!.DoctorName,
                    WardId = newOrder.WardId,
                    WardName = ward!.WardName,
                    OrderDate = newOrder.OrderDate,
                    Diagnosis = newOrder.Diagnosis,
                    Room = newOrder.Room
                };

                // Gán kết quả vào response
                response.Success = true;
                response.Data = orderResponse;
                response.Message = "Đã thêm thành công!";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi không mong muốn
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình thêm: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int orderId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chỉ định.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldOrder = new Order
                {
                    Id = order.Id,
                    PatientId = order.PatientId,
                    WardId = order.WardId,
                    DoctorId = order.DoctorId,
                    OrderDate = order.OrderDate,
                    Diagnosis = order.Diagnosis,
                    Room = order.Room
                };

                // Xoá chỉ định
                _unitOfWork.OrderRepository.Remove(order);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldOrder, null, nameof(Order), orderId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa chỉ định thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa chỉ định: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Order>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Order>>();

            try
            {
                var orders = await _unitOfWork.OrderRepository.GetAllAsync();

                response.Success = true;
                response.Data = orders;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách chỉ định.";
            }

            return response;
        }

        public async Task<ServiceResponse<Order?>> GetById(int orderId)
        {
            var response = new ServiceResponse<Order?>();

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy chỉ định.";
                return response;
            }

            response.Success = true;
            response.Data = order;
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<OrderDto>>> GetByPatientId(int patientId)
        {
            var response = new ServiceResponse<IEnumerable<OrderDto>>();

            try
            {
                var query = _unitOfWork.OrderRepository.GetListByAsync(x => x.PatientId == patientId, x => x.Doctor, x => x.Ward); // Thực hiện các hoạt động LINQ trước khi gọi `await`
                var orders = (await query).Select(x => new OrderDto 
                {
                    Id = x.Id, 
                    PatientId = x.PatientId, 
                    DoctorId = x.DoctorId, 
                    DoctorName = x.Doctor.DoctorName,
                    WardId = x.WardId,
                    WardName = x.Ward.WardName, 
                    OrderDate = x.OrderDate, 
                    Diagnosis = x.Diagnosis, 
                    Room = x.Room 
                }).ToList();

                response.Success = true;
                response.Data = orders;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách chỉ định theo mã bệnh nhân.";
            }

            return response;
        }

        public async Task<ServiceResponse<OrderDto>> Update(int orderId, OrderRequest request)
        {
            var response = new ServiceResponse<OrderDto>();

            try
            {
                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chỉ định.";
                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldOrder = new Order
                {
                    Id = order.Id,
                    PatientId = order.PatientId,
                    WardId = order.WardId,
                    DoctorId = order.DoctorId,
                    OrderDate = order.OrderDate,
                    Diagnosis = order.Diagnosis,
                    Room = order.Room
                };

                // Cập nhật thông tin chỉ định
                order.PatientId = request.PatientId;
                order.WardId = request.WardId;
                order.DoctorId = request.DoctorId;
                order.OrderDate = request.OrderDate;
                order.Diagnosis = request.Diagnosis;
                order.Room = request.Room;

                _unitOfWork.OrderRepository.Update(order);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldOrder, order, nameof(Order), orderId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                // Danh sách lỗi để gom các thông báo
                var errorMessages = new List<string>();

                var ward = await _unitOfWork.WardRepository.GetByIdAsync(order.WardId);
                if (ward == null)
                {
                    errorMessages.Add("Khoa phòng không tồn tại.");
                }

                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(order.DoctorId);
                if (doctor == null)
                {
                    errorMessages.Add("Bác sĩ không tồn tại.");
                }

                // Nếu có lỗi, trả về thông báo lỗi
                if (errorMessages.Any())
                {
                    response.Success = false;
                    response.Message = string.Join(", ", errorMessages); // Gộp tất cả thông báo lỗi
                    return response;
                }

                var orderResponse = new OrderDto()
                {
                    Id = order.Id,
                    PatientId = order.PatientId,
                    DoctorId = order.DoctorId,
                    DoctorName = doctor!.DoctorName,
                    WardId = order.WardId,
                    WardName = ward!.WardName,
                    OrderDate = order.OrderDate,
                    Diagnosis = order.Diagnosis,
                    Room = order.Room
                };

                response.Data = orderResponse;
                response.Success = true;
                response.Message = "Cập nhật thông tin chỉ định thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật chỉ định: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
