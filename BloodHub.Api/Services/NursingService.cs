using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface INursingService
    {
        Task<ServiceResponse<IEnumerable<Nursing>>> GetAll();
        Task<ServiceResponse<Nursing?>> GetById(int nursingId);
        Task<ServiceResponse<Nursing>> Add(NursingRequest request);
        Task<ServiceResponse<Nursing?>> Update(int nursingId, NursingRequest request);
        Task<ServiceResponse<bool>> Delete(int nursingId);
    }

    #endregion

    public class NursingService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : INursingService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Nursing>> Add(NursingRequest request)
        {
            var response = new ServiceResponse<Nursing>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                var duplicate = await _unitOfWork.NursingRepository.IsExists(0, request.NursingName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên điều dưỡng ''{request.NursingName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm nursing
                var newNursing = new Nursing()
                {
                    NursingName = request.NursingName,
                    IsHide = request.IsHide
                };
                await _unitOfWork.NursingRepository.AddAsync(newNursing);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newNursing;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int nursingId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var nursing = await _unitOfWork.NursingRepository.GetByIdAsync(nursingId);
                if (nursing == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy điều dưỡng.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldNursing = new Nursing
                {
                    Id = nursing.Id,
                    NursingName = nursing.NursingName,
                    IsHide = nursing.IsHide
                };

                // Xoá điều dưỡng
                _unitOfWork.NursingRepository.Remove(nursing);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldNursing, null, nameof(Nursing), nursingId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa điều dưỡng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa điều dưỡng: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Nursing>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Nursing>>();

            try
            {
                var nursings = await _unitOfWork.NursingRepository.GetAllAsync();

                response.Success = true;
                response.Data = nursings;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách điều dưỡng.";
            }

            return response;
        }

        public async Task<ServiceResponse<Nursing?>> GetById(int nursingId)
        {
            var response = new ServiceResponse<Nursing?>();

            var nursing = await _unitOfWork.NursingRepository.GetByIdAsync(nursingId);
            if (nursing == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy điều dưỡng.";
                return response;
            }

            response.Success = true;
            response.Data = nursing;
            return response;
        }

        public async Task<ServiceResponse<Nursing?>> Update(int nursingId, NursingRequest request)
        {
            var response = new ServiceResponse<Nursing?>();

            try
            {
                var nursing = await _unitOfWork.NursingRepository.GetByIdAsync(nursingId);
                if (nursing == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy điều dưỡng.";
                    return response;
                }

                bool duplicate = await _unitOfWork.NursingRepository.IsExists(nursingId, request.NursingName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên điều dưỡng ''{request.NursingName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldNursing = new Nursing
                {
                    Id = nursing.Id,
                    NursingName = nursing.NursingName,
                    IsHide = nursing.IsHide
                };

                // Cập nhật thông tin điều dưỡng
                nursing.NursingName = request.NursingName ?? nursing.NursingName;
                nursing.IsHide = request.IsHide;

                _unitOfWork.NursingRepository.Update(nursing);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldNursing, nursing, nameof(Nursing), nursingId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = nursing;
                response.Success = true;
                response.Message = "Cập nhật thông tin điều dưỡng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật điều dưỡng: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
