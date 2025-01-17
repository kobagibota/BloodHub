using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IPatientService
    {
        Task<ServiceResponse<IEnumerable<Patient>>> GetAll();
        Task<ServiceResponse<Patient?>> GetById(int patientId);
        Task<ServiceResponse<Patient>> Add(PatientRequest request);
        Task<ServiceResponse<Patient?>> Update(int patientId, PatientRequest request);
        Task<ServiceResponse<bool>> Delete(int patientId);
    }

    #endregion

    public class PatientService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IPatientService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Patient>> Add(PatientRequest request)
        {
            var response = new ServiceResponse<Patient>();

            try
            {
                if (request == null)
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";

                    return response;
                }

                // thêm patient
                var newPatient = new Patient()
                {
                    MedicalId = request.MedicalId,
                    PatientName = request.PatientName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Address = request.Address,
                    BloodGroup = request.BloodGroup,
                    Rhesus = request.Rhesus
                };
                await _unitOfWork.PatientRepository.AddAsync(newPatient);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newPatient;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int patientId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bệnh nhân.";
                    return response;
                }

                // Lưu thông tin trước khi xoá
                var oldPatient = new Patient
                {
                    Id = patient.Id,
                    MedicalId = patient.MedicalId,
                    PatientName = patient.PatientName,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender,
                    Address = patient.Address,
                    BloodGroup = patient.BloodGroup,
                    Rhesus = patient.Rhesus
                };

                // Xoá bệnh nhân
                _unitOfWork.PatientRepository.Remove(patient);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldPatient, null, nameof(Patient), patientId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa bệnh nhân thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa bệnh nhân: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Patient>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<Patient>>();

            try
            {
                var patients = await _unitOfWork.PatientRepository.GetAllAsync();

                response.Success = true;
                response.Data = patients;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách bệnh nhân.";
            }

            return response;
        }

        public async Task<ServiceResponse<Patient?>> GetById(int patientId)
        {
            var response = new ServiceResponse<Patient?>();

            var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
            if (patient == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy bệnh nhân.";
                return response;
            }

            response.Success = true;
            response.Data = patient;
            return response;
        }

        public async Task<ServiceResponse<Patient?>> Update(int patientId, PatientRequest request)
        {
            var response = new ServiceResponse<Patient?>();

            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId);
                if (patient == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy bệnh nhân.";
                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldPatient = new Patient
                {
                    Id = patient.Id,
                    MedicalId = patient.MedicalId,
                    PatientName = patient.PatientName,
                    DateOfBirth = patient.DateOfBirth,
                    Gender = patient.Gender,
                    Address = patient.Address,
                    BloodGroup = patient.BloodGroup,
                    Rhesus = patient.Rhesus
                };

                // Cập nhật thông tin bệnh nhân
                patient.MedicalId = request.MedicalId ?? patient.MedicalId;
                patient.PatientName = request.PatientName ?? patient.PatientName;
                patient.DateOfBirth = request.DateOfBirth;
                patient.Gender = request.Gender;
                patient.Address = request.Address ?? patient.Address;
                patient.BloodGroup = request.BloodGroup;
                patient.Rhesus = request.Rhesus;

                _unitOfWork.PatientRepository.Update(patient);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi                 
                var saveLog = await _changeLogService.Add(oldPatient, patient, nameof(Patient), patientId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Data = patient;
                response.Success = true;
                response.Message = "Cập nhật thông tin bệnh nhân thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật bệnh nhân: {ex.Message}";
            }

            return response;
        }

        #endregion
    }
}
