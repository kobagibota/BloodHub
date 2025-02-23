using BloodHub.Shared.DTOs;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IShiftDetailService
    {
        Task<ServiceResponse<IEnumerable<ShiftDetailDto>>> GetListByShiftId(int shitId);
        //Task<ServiceResponse<ShiftDetail?>> GetById(int shiftDetailId);
        //Task<ServiceResponse<bool>> Delete(int shiftDetailId);
    }

    #endregion

    public class ShiftDetailService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IShiftDetailService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        //public async Task<ServiceResponse<ShiftDetail>> Add(ShiftDetailRequest request)
        //{
        //    var response = new ServiceResponse<ShiftDetail>();

        //    try
        //    {
        //        if (request == null)
        //        {
        //            response.Success = false;
        //            response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

        //            return response;
        //        }

        //        // thêm shiftDetail
        //        var newShiftDetail = new ShiftDetail()
        //        {
        //            MedicalId = request.MedicalId,
        //            ShiftDetailName = request.ShiftDetailName,
        //            DateOfBirth = request.DateOfBirth,
        //            Gender = request.Gender,
        //            Address = request.Address,
        //            BloodGroup = request.BloodGroup,
        //            Rhesus = request.Rhesus
        //        };
        //        await _unitOfWork.ShiftDetailRepository.AddAsync(newShiftDetail);
        //        await _unitOfWork.SaveAsync();

        //        response.Success = true;
        //        response.Data = newShiftDetail;
        //        response.Message = $"Đã thêm thành công!";
        //    }
        //    catch (Exception)
        //    {
        //        response.Success = false;
        //        response.Message = "Xảy ra lỗi trong quá trình thêm.";
        //    }

        //    return response;
        //}

        //public async Task<ServiceResponse<bool>> Delete(int shiftDetailId)
        //{
        //    var response = new ServiceResponse<bool>();

        //    try
        //    {
        //        var shiftDetail = await _unitOfWork.ShiftDetailRepository.GetByIdAsync(shiftDetailId);
        //        if (shiftDetail == null)
        //        {
        //            response.Success = false;
        //            response.Message = "Không tìm thấy tồn kho.";
        //            return response;
        //        }

        //        // Lưu thông tin trước khi xoá
        //        var oldShiftDetail = new ShiftDetail
        //        {
        //            Id = shiftDetail.Id,
        //            MedicalId = shiftDetail.MedicalId,
        //            ShiftDetailName = shiftDetail.ShiftDetailName,
        //            DateOfBirth = shiftDetail.DateOfBirth,
        //            Gender = shiftDetail.Gender,
        //            Address = shiftDetail.Address,
        //            BloodGroup = shiftDetail.BloodGroup,
        //            Rhesus = shiftDetail.Rhesus
        //        };

        //        // Xoá tồn kho
        //        _unitOfWork.ShiftDetailRepository.Remove(shiftDetail);
        //        await _unitOfWork.SaveAsync();

        //        // Ghi log thay đổi  
        //        var saveLog = await _changeLogService.Add(oldShiftDetail, null, nameof(ShiftDetail), shiftDetailId, ChangeType.Delete);
        //        if (!saveLog.Success)
        //        {
        //            response.Success = false;
        //            response.Message = saveLog.Message;
        //            return response;
        //        }

        //        response.Success = true;
        //        response.Data = true;
        //        response.Message = "Xóa tồn kho thành công.";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"Xảy ra lỗi trong quá trình xóa tồn kho: {ex.Message}";
        //    }

        //    return response;
        //}

        public async Task<ServiceResponse<IEnumerable<ShiftDetailDto>>> GetListByShiftId(int shiftId)
        {
            var response = new ServiceResponse<IEnumerable<ShiftDetailDto>>();

            try
            {
                var shiftDetails = await _unitOfWork.ShiftDetailRepository.GetListByAsync(x => x.ShiftId == shiftId, include: x => x.Product);

                response.Success = true;
                response.Data = shiftDetails.ConvertToDto();
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách tồn kho.";
            }

            return response;
        }

        //public async Task<ServiceResponse<ShiftDetail?>> GetById(int shiftDetailId)
        //{
        //    var response = new ServiceResponse<ShiftDetail?>();

        //    var shiftDetail = await _unitOfWork.ShiftDetailRepository.GetByIdAsync(shiftDetailId);
        //    if (shiftDetail == null)
        //    {
        //        response.Success = false;
        //        response.Message = "Không tìm thấy tồn kho.";
        //        return response;
        //    }

        //    response.Success = true;
        //    response.Data = shiftDetail;
        //    return response;
        //}

        //public async Task<ServiceResponse<ShiftDetail?>> Update(int shiftDetailId, ShiftDetailRequest request)
        //{
        //    var response = new ServiceResponse<ShiftDetail?>();

        //    try
        //    {
        //        var shiftDetail = await _unitOfWork.ShiftDetailRepository.GetByIdAsync(shiftDetailId);
        //        if (shiftDetail == null)
        //        {
        //            response.Success = false;
        //            response.Message = "Không tìm thấy tồn kho.";
        //            return response;
        //        }

        //        // Lưu thông tin trước khi thay đổi
        //        var oldShiftDetail = new ShiftDetail
        //        {
        //            Id = shiftDetail.Id,
        //            MedicalId = shiftDetail.MedicalId,
        //            ShiftDetailName = shiftDetail.ShiftDetailName,
        //            DateOfBirth = shiftDetail.DateOfBirth,
        //            Gender = shiftDetail.Gender,
        //            Address = shiftDetail.Address,
        //            BloodGroup = shiftDetail.BloodGroup,
        //            Rhesus = shiftDetail.Rhesus
        //        };

        //        // Cập nhật thông tin tồn kho
        //        shiftDetail.MedicalId = request.MedicalId ?? shiftDetail.MedicalId;
        //        shiftDetail.ShiftDetailName = request.ShiftDetailName ?? shiftDetail.ShiftDetailName;
        //        shiftDetail.DateOfBirth = request.DateOfBirth;
        //        shiftDetail.Gender = request.Gender;
        //        shiftDetail.Address = request.Address ?? shiftDetail.Address;
        //        shiftDetail.BloodGroup = request.BloodGroup;
        //        shiftDetail.Rhesus = request.Rhesus;

        //        _unitOfWork.ShiftDetailRepository.Update(shiftDetail);
        //        await _unitOfWork.SaveAsync();

        //        // Ghi log thay đổi                 
        //        var saveLog = await _changeLogService.Add(oldShiftDetail, shiftDetail, nameof(ShiftDetail), shiftDetailId, ChangeType.Update);
        //        if (!saveLog.Success)
        //        {
        //            response.Success = false;
        //            response.Message = saveLog.Message;
        //            return response;
        //        }

        //        response.Data = shiftDetail;
        //        response.Success = true;
        //        response.Message = "Cập nhật thông tin tồn kho thành công.";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"Xảy ra lỗi trong quá trình cập nhật tồn kho: {ex.Message}";
        //    }

        //    return response;
        //}

        #endregion
    }
}
