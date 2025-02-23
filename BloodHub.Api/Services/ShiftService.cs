using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IShiftService
    {
        Task<ServiceResponse<IEnumerable<ShiftDto>>> GetAll();
        Task<ServiceResponse<ShiftDto?>> GetById(int shiftId);
        Task<ServiceResponse<ShiftDto>> Add(ShiftRequest request);
        Task<ServiceResponse<ShiftDto?>> Update(int shiftId, ShiftRequest request);
        Task<ServiceResponse<bool>> Delete(int shiftId);

        Task<ServiceResponse<bool>> Handover(int shiftId, ShiftHandoverRequest request);
        Task<ServiceResponse<bool>> ConfirmHandover(int shiftId, ShiftConfirmHandoverRequest request);
    }

    #endregion

    public class ShiftService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IShiftService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<ShiftDto>> Add(ShiftRequest request)
        {
            var response = new ServiceResponse<ShiftDto>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                var duplicate = await _unitOfWork.ShiftRepository.IsExists(s => s.ShiftName == request.ShiftName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên ca trực ''{request.ShiftName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm shift
                var newShift = new Shift()
                {
                    ShiftName = request.ShiftName,
                    ShiftStart = request.ShiftStart,
                    Status = ShiftStatus.Pending
                };
                await _unitOfWork.ShiftRepository.AddAsync(newShift);
                await _unitOfWork.SaveAsync();

                // Lấy ID của Shift vừa thêm
                var shiftId = newShift.Id;

                // Thêm danh sách ShiftUser vào bảng ShiftUser
                if (request.UserIds != null && request.UserIds.Count > 0)
                {
                    foreach (var userId in request.UserIds)
                    {
                        var shiftUser = new ShiftUser
                        {
                            ShiftId = shiftId,
                            UserId = userId
                        };
                        await _unitOfWork.ShiftUserRepository.AddAsync(shiftUser);
                    }
                    await _unitOfWork.SaveAsync();
                }

                // Tải thông tin User cho ShiftUser từ UnitOfWork
                var shiftEntity = await _unitOfWork.ShiftRepository.GetAsync(
                    filter: s => s.Id == newShift.Id,
                    include: q => q.Include(s => s.ShiftUsers).ThenInclude(su => su.User));

                var shiftUsers = shiftEntity.First().ShiftUsers;

                // Map Shift sang ShiftDto để trả về
                var shiftUserDtos = shiftUsers.Select(su => new ShiftUserDto
                {
                    UserId = su.UserId,
                    ShortName = su.User?.ShortName ?? string.Empty
                });

                response.Success = true;
                response.Data = newShift.ConvertToDto(shiftUserDtos);
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int shiftId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var shift = await _unitOfWork.ShiftRepository.GetByIdAsync(shiftId);
                if (shift == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ca trực.";
                    return response;
                }

                if (shift.Status != ShiftStatus.Pending)
                {
                    response.Success = false;
                    response.Message = "Đang trong ca trực nên không thể xoá.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldShift = new Shift
                {
                    Id = shift.Id,
                    ShiftName = shift.ShiftName,
                    ShiftStart = shift.ShiftStart,
                    ShiftEnd = shift.ShiftEnd,
                    HandoverTime = shift.HandoverTime,
                    ReceivedShiftId = shift.ReceivedShiftId,
                    HandBy = shift.HandBy,
                    ReceivedBy = shift.ReceivedBy,
                    Note = shift.Note,
                    Status = shift.Status
                };

                // Xoá ca trực
                _unitOfWork.ShiftRepository.Remove(shift);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldShift, null, nameof(Shift), shiftId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa ca trực thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa ca trực: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ShiftDto>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<ShiftDto>>();

            try
            {
                var shifts = await _unitOfWork.ShiftRepository.GetAsync(include: q => q.Include(s => s.ShiftUsers).ThenInclude(su => su.User));

                // Map dữ liệu từ Shift sang ShiftDto
                var shiftDtos = shifts.Select(s =>
                {
                    var shiftUserDto = s.ShiftUsers.Select(su => new ShiftUserDto
                    {
                        UserId = su.UserId,
                        ShortName = su.User?.ShortName ?? string.Empty
                    });

                    return s.ConvertToDto(shiftUserDto);
                });

                response.Success = true;
                response.Data = shiftDtos;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách ca trực.";
            }

            return response;
        }

        public async Task<ServiceResponse<ShiftDto?>> GetById(int shiftId)
        {
            var response = new ServiceResponse<ShiftDto?>();

            var shift = await _unitOfWork.ShiftRepository.GetAsync(
                filter: s => s.Id == shiftId,
                include: q => q.Include(s => s.ShiftUsers).ThenInclude(su => su.User));
            
            var shiftEntity = shift.FirstOrDefault();
            if (shiftEntity == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy ca trực.";
                return response;
            }

            // Map ShiftUser sang ShiftUserDto
            var shiftUserDtos = shiftEntity.ShiftUsers.Select(su => new ShiftUserDto
            {
                UserId = su.UserId,
                ShortName = su.User?.ShortName ?? string.Empty
            });

            // Map Shift sang ShiftDto
            var shiftDto = shiftEntity.ConvertToDto(shiftUserDtos);

            response.Success = true;
            response.Data = shiftDto;
            return response;
        }

        public async Task<ServiceResponse<ShiftDto?>> Update(int shiftId, ShiftRequest request)
        {
            var response = new ServiceResponse<ShiftDto?>();

            try
            {
                var shift = await _unitOfWork.ShiftRepository.GetAsync(
                    filter: s => s.Id == shiftId,
                    include: q => q.Include(s => s.ShiftUsers).ThenInclude(su => su.User));

                var shiftEntity = shift.FirstOrDefault();
                if (shiftEntity == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ca trực.";
                    return response;
                }

                bool duplicate = await _unitOfWork.ShiftRepository.IsExists(d => d.ShiftName == request.ShiftName && d.Id != shiftId);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên ca trực ''{request.ShiftName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldShift = new Shift
                {
                    Id = shiftEntity.Id,
                    ShiftName = shiftEntity.ShiftName,
                    ShiftStart = shiftEntity.ShiftStart,
                };

                // Cập nhật thông tin ca trực
                shiftEntity.ShiftName = request.ShiftName ?? shiftEntity.ShiftName;
                shiftEntity.ShiftStart = request.ShiftStart;

                // Cập nhật danh sách ShiftUser
                var currentShiftUsers = shiftEntity.ShiftUsers.ToList();
                var newShiftUsers = request.UserIds;

                // Xóa những ShiftUser không còn nằm trong danh sách mới
                foreach (var shiftUser in currentShiftUsers)
                {
                    if (!newShiftUsers.Contains(shiftUser.UserId))
                    {
                        _unitOfWork.ShiftUserRepository.Remove(shiftUser);
                    }
                }

                // Thêm những ShiftUser mới vào danh sách
                foreach (var userId in newShiftUsers)
                {
                    if (!currentShiftUsers.Any(su => su.UserId == userId))
                    {
                        var newShiftUser = new ShiftUser
                        {
                            ShiftId = shiftEntity.Id,
                            UserId = userId
                        };
                        await _unitOfWork.ShiftUserRepository.AddAsync(newShiftUser);
                    }
                }

                _unitOfWork.ShiftRepository.Update(shiftEntity);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldShift, shiftEntity, nameof(Shift), shiftId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                // Lấy lại danh sách ShiftUsers sau khi cập nhật
                var updatedShift = await _unitOfWork.ShiftRepository.GetAsync(
                    filter: s => s.Id == shiftEntity.Id,
                    include: q => q.Include(s => s.ShiftUsers)
                                   .ThenInclude(su => su.User)
                );

                var updatedShiftEntity = updatedShift.First();

                var shiftUserDtos = updatedShiftEntity.ShiftUsers.Select(su => new ShiftUserDto
                {
                    UserId = su.UserId,
                    ShortName = su.User?.ShortName ?? string.Empty
                });

                response.Data = updatedShiftEntity.ConvertToDto(shiftUserDtos);
                response.Success = true;
                response.Message = "Cập nhật thông tin ca trực thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật ca trực: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Handover(int shiftId, ShiftHandoverRequest request)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var shift = await _unitOfWork.ShiftRepository.GetByIdAsync(shiftId);

                if (shift == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ca trực.";
                    return response;
                }

                if (shift.Status != ShiftStatus.InProgress)
                {
                    response.Success = false;
                    response.Message = "Không trong ca trực nên không thể giao ca.";
                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldShift = new Shift
                {
                    ShiftEnd = shift.ShiftEnd,
                    UserHand = shift.UserHand,
                    ReceivedShiftId = shift.ReceivedShiftId,
                    Note = shift.Note,
                    Status = shift.Status
                };

                // Cập nhật thông tin ca trực
                shift.ShiftEnd = request.ShiftEnd;
                shift.HandBy = request.HandBy;
                shift.ReceivedShiftId = request.ReceivedShiftId;
                shift.Note = request.Note;
                shift.Status = ShiftStatus.Transferred;               

                _unitOfWork.ShiftRepository.Update(shift);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldShift, shift, nameof(Shift), shiftId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = true;
                response.Success = true;
                response.Message = "Cập nhật thông tin giao ca trực thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật thông tin giao ca trực: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ConfirmHandover(int shiftId, ShiftConfirmHandoverRequest request)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var shift = await _unitOfWork.ShiftRepository.GetByIdAsync(shiftId);

                if (shift == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy ca trực.";
                    return response;
                }

                if (shift.Status != ShiftStatus.Transferred)
                {
                    response.Success = false;
                    response.Message = "Vui lòng cập nhật thông tin bàn giao trước khi xác nhận giao ca.";
                    return response;
                }

                // Cập nhật thông tin ca giao
                shift.HandoverTime = request.HandoverTime;
                shift.ReceivedBy = request.ReceivedBy;
                shift.Status = ShiftStatus.Completed;

                _unitOfWork.ShiftRepository.Update(shift);

                // Cập nhật trạng thái cho ca nhận
                var receivedShift = await _unitOfWork.ShiftRepository.GetByIdAsync(shift.ReceivedShiftId ?? 0);
                if (receivedShift != null)
                {
                    receivedShift.Status = ShiftStatus.InProgress;
                    _unitOfWork.ShiftRepository.Update(receivedShift);
                }

                // Lưu thông tin
                await _unitOfWork.SaveAsync();

                response.Data = true;
                response.Success = true;
                response.Message = "Xác nhận giao ca trực thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật thông tin giao ca trực: {ex.Message}";
            }

            return response;
        }


        #endregion
    }
}
