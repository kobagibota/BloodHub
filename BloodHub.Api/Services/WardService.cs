using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IWardService
    {
        Task<ServiceResponse<IEnumerable<Ward>>> GetAll();
        Task<ServiceResponse<Ward?>> GetById(int wardId);
        Task<ServiceResponse<Ward>> Add(WardRequest request);
        Task<ServiceResponse<Ward?>> Update(int wardId, WardRequest request);
        Task<ServiceResponse<bool>> Delete(int wardId);
    }

    #endregion

    public class WardService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IWardService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Ward>> Add(WardRequest request)
        {
            var response = new ServiceResponse<Ward>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                var duplicate = await _unitOfWork.WardRepository.IsExists(0, request.WardName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên khoa phòng ''{request.WardName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm ward
                var newWard = new Ward()
                {
                    WardName = request.WardName
                };
                await _unitOfWork.WardRepository.AddAsync(newWard);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newWard;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int wardId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var ward = await _unitOfWork.WardRepository.GetByIdAsync(wardId);
                if (ward == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy khoa phòng.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldWard = new Ward
                {
                    Id = ward.Id,
                    WardName = ward.WardName
                };

                // Xoá khoa phòng
                _unitOfWork.WardRepository.Remove(ward);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldWard, null, nameof(Ward), wardId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa khoa phòng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa khoa phòng: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Ward>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Ward>>();

            try
            {
                var wards = await _unitOfWork.WardRepository.GetAllAsync();

                response.Success = true;
                response.Data = wards;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách khoa phòng.";
            }

            return response;
        }

        public async Task<ServiceResponse<Ward?>> GetById(int wardId)
        {
            var response = new ServiceResponse<Ward?>();

            var ward = await _unitOfWork.WardRepository.GetByIdAsync(wardId);
            if (ward == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy khoa phòng.";
                return response;
            }

            response.Success = true;
            response.Data = ward;
            return response;
        }

        public async Task<ServiceResponse<Ward?>> Update(int wardId, WardRequest request)
        {
            var response = new ServiceResponse<Ward?>();

            try
            {
                var ward = await _unitOfWork.WardRepository.GetByIdAsync(wardId);
                if (ward == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy khoa phòng.";
                    return response;
                }

                bool duplicate = await _unitOfWork.WardRepository.IsExists(wardId, request.WardName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên khoa phòng ''{request.WardName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldWard = new Ward
                {
                    Id = ward.Id,
                    WardName = ward.WardName
                };

                // Cập nhật thông tin khoa phòng
                ward.WardName = request.WardName ?? ward.WardName;

                _unitOfWork.WardRepository.Update(ward);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldWard, ward, nameof(Ward), wardId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = ward;
                response.Success = true;
                response.Message = "Cập nhật thông tin khoa phòng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật khoa phòng: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
