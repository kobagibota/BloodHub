using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IDoctorService
    {
        Task<ServiceResponse<IEnumerable<Doctor>>> GetAll();
        Task<ServiceResponse<Doctor?>> GetById(int doctorId);
        Task<ServiceResponse<Doctor>> Add(DoctorRequest request);
        Task<ServiceResponse<Doctor?>> Update(int doctorId, DoctorRequest request);
        Task<ServiceResponse<bool>> Delete(int doctorId);
    }

    #endregion

    public class DoctorService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IDoctorService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Doctor>> Add(DoctorRequest request)
        {
            var response = new ServiceResponse<Doctor>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                var duplicate = await _unitOfWork.DoctorRepository.IsExists(0, request.DoctorName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên bác sĩ ''{request.DoctorName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm doctor
                var newDoctor = new Doctor()
                {
                    DoctorName = request.DoctorName,
                    IsHide = request.IsHide
                };
                await _unitOfWork.DoctorRepository.AddAsync(newDoctor);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newDoctor;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int doctorId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bác sĩ.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldDoctor = new Doctor
                {
                    Id = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    IsHide = doctor.IsHide
                };

                // Xoá bác sĩ
                _unitOfWork.DoctorRepository.Remove(doctor);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldDoctor, null, nameof(Doctor), doctorId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa bác sĩ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa bác sĩ: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Doctor>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Doctor>>();

            try
            {
                var doctors = await _unitOfWork.DoctorRepository.GetAllAsync();

                response.Success = true;
                response.Data = doctors;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách bác sĩ.";
            }

            return response;
        }

        public async Task<ServiceResponse<Doctor?>> GetById(int doctorId)
        {
            var response = new ServiceResponse<Doctor?>();

            var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy bác sĩ.";
                return response;
            }

            response.Success = true;
            response.Data = doctor;
            return response;
        }

        public async Task<ServiceResponse<Doctor?>> Update(int doctorId, DoctorRequest request)
        {
            var response = new ServiceResponse<Doctor?>();

            try
            {
                var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId);
                if (doctor == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bác sĩ.";
                    return response;
                }

                bool duplicate = await _unitOfWork.DoctorRepository.IsExists(doctorId, request.DoctorName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên bác sĩ ''{request.DoctorName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldDoctor = new Doctor
                {
                    Id = doctor.Id,
                    DoctorName = doctor.DoctorName,
                    IsHide = doctor.IsHide
                };

                // Cập nhật thông tin bác sĩ
                doctor.DoctorName = request.DoctorName ?? doctor.DoctorName;
                doctor.IsHide = request.IsHide;

                _unitOfWork.DoctorRepository.Update(doctor);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldDoctor, doctor, nameof(Doctor), doctorId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = doctor;
                response.Success = true;
                response.Message = "Cập nhật thông tin bác sĩ thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật bác sĩ: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
