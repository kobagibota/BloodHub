using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Respones;
using BloodHub.Shared.Extentions;
using BloodHub.Api.Extensions;
using System.Text.Json;
using System.Collections;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IChangeLogService
    {
        Task<ServiceResponse<IEnumerable<ChangeLogDto>>> GetAll();
        Task<ServiceResponse<ChangeLogDto?>> GetById(int changeLogId);
        Task<ServiceResponse<bool>> Add<T>(T oldObject, T newObject, string tableName, int recordId, ChangeType changeType);
        Task<ServiceResponse<bool>> Delete(int changeLogId);
        Task<ServiceResponse<bool>> DeleteMulti(IEnumerable<int> changeLogIds);
    }

    #endregion

    public class ChangeLogService(IUnitOfWork unitOfWork, RequestInfoProvider infoProvider) : IChangeLogService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly RequestInfoProvider _infoProvider = infoProvider;

        #endregion

        #region Methods

        public async Task<ServiceResponse<bool>> Add<T>(T oldObject, T newObject, string tableName, int recordId, ChangeType changeType)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                // So sánh và lấy các thay đổi
                var changes = GetChanges(oldObject, newObject, changeType);

                if (changes.Count == 0)
                {
                    response.Success = true;
                    response.Message = "Không có thay đổi nào để ghi log.";
                    response.Data = true;
                    return response;
                }

                var userId = _infoProvider.GetUserIdFromToken();
                var ipAddress = _infoProvider.GetIpAddress();
                var userAgent = _infoProvider.GetUserAgent();

                // Gộp các thay đổi vào một chuỗi
                var changeDetails = JsonSerializer.Serialize(changes);
                var changeLog = new ChangeLog
                {
                    UserId = userId,
                    TableName = tableName,
                    RecordId = recordId,
                    ChangeType = changeType,
                    ChangeDetails = changeDetails,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                await _unitOfWork.ChangeLogRepository.AddAsync(changeLog);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Message = "Ghi log thành công.";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi khi ghi log: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int id)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var changeLog = await _unitOfWork.ChangeLogRepository.GetByIdAsync(id);
                if (changeLog == null)
                {
                    response.Success = false;
                    response.Message = "Log không tồn tại.";
                    return response;
                }
                _unitOfWork.ChangeLogRepository.Remove(changeLog);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi khi xóa log: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteMulti(IEnumerable<int> changeLogIds)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var changeLogs = new List<ChangeLog>();
                foreach (var id in changeLogIds)
                {
                    var changeLog = await _unitOfWork.ChangeLogRepository.GetByIdAsync(id);
                    if (changeLog != null)
                    {
                        changeLogs.Add(changeLog);
                    }
                }
                _unitOfWork.ChangeLogRepository.RemoveRange(changeLogs);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi khi xóa các log: {ex.Message}";
            }
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<ChangeLogDto>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<ChangeLogDto>>();

            try
            {
                var changeLogs = await _unitOfWork.ChangeLogRepository.GetAllAsync();
                var userIds = changeLogs.Select(cl => cl.UserId).Distinct();
                var users = await _unitOfWork.UserRepository.GetListByAsync(u => u.Equals(userIds));

                // Chuyển đổi ChangeLogs sang ChangeLogDtos  
                var changeLogDtos = changeLogs.ConvertToDto(users);

                response.Success = true;
                response.Data = changeLogDtos;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách log.";
            }

            return response;
        }

        public async Task<ServiceResponse<ChangeLogDto?>> GetById(int changeLogId)
        {
            var response = new ServiceResponse<ChangeLogDto?>();

            var changeLog = await _unitOfWork.ChangeLogRepository.GetByIdAsync(changeLogId);
            if (changeLog == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy bác sĩ.";

                return response;
            }

            string fullname = await _unitOfWork.UserRepository.GetFullNameById(changeLog.UserId);
            // Chuyển đổi ChangeLogs sang ChangeLogDtos  
            ChangeLogDto changeLogDto = changeLog.ConvertToDto(fullname);

            response.Success = true;
            response.Data = changeLogDto;
            return response;
        }

        #endregion

        #region Helpers

        private List<ChangeLogDetail> GetChanges<T>(T oldObject, T newObject, ChangeType changeType)
        {
            var changes = new List<ChangeLogDetail>();

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                // Bỏ qua các thuộc tính có ForeignKeyAttribute hoặc là collection
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType)) //(Attribute.IsDefined(property, typeof(ForeignKeyAttribute)) ||
                {
                    continue;
                }

                var oldValue = property.GetValue(oldObject)?.ToString();
                var newValue = newObject != null ? property.GetValue(newObject)?.ToString() : null;

                if (oldValue != newValue)
                {
                    changes.Add(new ChangeLogDetail
                    {
                        ColumnName = property.Name,
                        OldValue = oldValue,
                        NewValue = changeType == ChangeType.Update ? newValue : "Đã xoá"
                    });
                }
            }

            return changes;
        }


        #endregion
    }
}
