using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Respones;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IRoleService
    {
        Task<ServiceResponse<IEnumerable<string>>> GetAll();
        Task<ServiceResponse<Role>> GetById(int id);
        Task<ServiceResponse<Role>> Add(string roleName);
        Task<ServiceResponse<bool>> Delete(string roleName);
    }

    #endregion

    public class RoleService(IUnitOfWork unitOfWork, IChangeLogService changeLogService) : IRoleService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;

        #endregion

        #region Methods

        public async Task<ServiceResponse<Role>> Add(string roleName)
        {
            var response = new ServiceResponse<Role>();

            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    response.Success = false;
                    response.Message = "Tên vai trò không hợp lệ!.";

                    return response;
                }

                var duplicate = await _unitOfWork.RoleRepository.IsExists(roleName);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên vai trò ''{roleName}'' đã tồn tại. \n Mời bạn xem lại.";

                    return response;
                }

                // thêm role
                var newRole = new Role()
                {
                    Name = roleName
                };
                await _unitOfWork.RoleRepository.AddAsync(newRole);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Data = newRole;
                response.Message = $"Đã thêm thành công!";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình thêm.";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(string roleName)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var role = await _unitOfWork.RoleRepository.GetByName(roleName);
                if (role == null)
                {
                    response.Success = false;
                    response.Message = $"Vài trò ''{roleName}'' không tồn tại.";
                    return response;
                }

                // Xoá vai trò
                _unitOfWork.RoleRepository.Remove(role);
                await _unitOfWork.SaveAsync();

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(roleName, null, nameof(Role), role.Id, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa vai trò thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình xóa vai trò: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<string>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<string>>();

            try
            {
                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                var roleNames = roles.Where(r => r.Name != null).Select(r => r.Name!).ToList();

                response.Success = true;
                response.Data = roleNames;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách vai trò.";
            }

            return response;
        }

        public async Task<ServiceResponse<Role>> GetById(int id)
        {
            var response = new ServiceResponse<Role>();

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (role == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy vai trò.";
                return response;
            }

            response.Success = true;
            response.Data = role;
            return response;
        }

        #endregion
    }
}
