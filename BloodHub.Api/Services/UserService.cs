using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;
using BloodHub.Shared.Respones;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Api.Services
{
    #region Interface

    public interface IUserService
    {
        Task<ServiceResponse<IEnumerable<UserDto>>> GetAll();
        Task<ServiceResponse<UserDto?>> GetById(int userId);
        Task<ServiceResponse<UserDto>> Add(UserRequest request);
        Task<ServiceResponse<UserDto>> Update(int userId, UserRequest request);
        Task<ServiceResponse<bool>> Delete(int userId);

        Task<ServiceResponse<IEnumerable<UserDto>>> GetAvailableUsersForShift();
        Task<ServiceResponse<bool>> ToggleActiveAsync(int userId);
    }

    #endregion

    public class UserService(IUnitOfWork unitOfWork, IChangeLogService changeLogService, 
        UserManager<User> userManager, RoleManager<Role> roleManager) : IUserService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly RoleManager<Role> _roleManager = roleManager;

        #endregion

        #region Methods

        public async Task<ServiceResponse<UserDto>> Add(UserRequest request)
        {
            var response = new ServiceResponse<UserDto>();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";
                    return response;
                }

                // Kiểm tra tên người dùng đã tồn tại chưa
                var existingUser = await _userManager.FindByNameAsync(request.UserName);
                if (existingUser != null)
                {
                    response.Success = false;
                    response.Message = $"Tên người dùng '{request.UserName}' đã tồn tại. Mời bạn xem lại.";
                    return response;
                }

                // Tạo user mới
                var newUser = new User
                {
                    UserName = request.UserName,
                    FullName = request.FullName,
                    IsActive = request.IsActive,
                    ContactInfo = request.ContactInfo,
                    CreatedAt = DateTime.UtcNow
                };

                // Tạo user với mật khẩu
                var result = await _userManager.CreateAsync(newUser, request.Password);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                    return response;
                }

                // Gán vai trò nếu có
                var rolesToAssign = request.Roles?.Any() == true ? request.Roles : new List<string> { "User" }; // Mặc định "User" nếu danh sách rỗng
                var roleResult = await _userManager.AddToRolesAsync(newUser, rolesToAssign);

                if (!roleResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = $"Lỗi khi gán vai trò: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}";
                    return response;
                }

                // Lấy danh sách vai trò thực tế
                var roles = await _userManager.GetRolesAsync(newUser);

                response.Success = true;
                response.Data = newUser.ConvertToUserDto(roles.ToList());
                response.Message = "Đã thêm thành công!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình thêm: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Delete(int userId)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return CreateErrorResponse("Không tìm thấy người dùng.");
                }

                // Lưu thông tin trước khi xoá
                var oldUser = new User
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    ContactInfo = user.ContactInfo,
                    IsActive = user.IsActive
                };

                // Xoá người dùng
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    return CreateErrorResponse(string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                // Ghi log thay đổi  
                var saveLog = await _changeLogService.Add(oldUser, null, nameof(User), userId, ChangeType.Delete);
                if (!saveLog.Success)
                {
                    return CreateErrorResponse(saveLog.Message);
                }

                response.Success = true;
                response.Data = true;
                response.Message = "Xóa người dùng thành công.";
            }
            catch (Exception ex)
            {
                response = CreateErrorResponse($"Xảy ra lỗi trong quá trình xóa người dùng: {ex.Message}");
            }

            return response;
        }

        private ServiceResponse<bool> CreateErrorResponse(string message)
        {
            return new ServiceResponse<bool>
            {
                Success = false,
                Data = false,
                Message = message
            };
        }

        public async Task<ServiceResponse<IEnumerable<UserDto>>> GetAll()
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();

            try
            {
                // Lấy danh sách Users cùng với UserRoles và Role
                var users = await _unitOfWork.UserRepository.GetAsync();

                var userRolesDict = await _roleManager.Roles
                        .SelectMany(r => r.UserRoles, (r, ur) => new { ur.UserId, RoleName = r.Name })
                        .GroupBy(ur => ur.UserId)
                        .ToDictionaryAsync(g => g.Key, g => g.Select(ur => ur.RoleName).ToList());

                // Sử dụng ConvertToUserDto() để chuyển đổi danh sách
                var userDtos = users.ConvertToUserDto(userRolesDict!);

                response.Success = true;
                response.Data = userDtos;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy danh sách người dùng.";
            }

            return response;
        }

        public async Task<ServiceResponse<UserDto?>> GetById(int userId)
        {
            var response = new ServiceResponse<UserDto?>();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy người dùng.";
                return response;
            }

            var roles = await _userManager.GetRolesAsync(user);

            response.Success = true;
            response.Data = user.ConvertToUserDto(roles.ToList());
            return response;
        }

        public async Task<ServiceResponse<UserDto>> Update(int userId, UserRequest request)
        {
            var response = new ServiceResponse<UserDto>();

            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy người dùng.";
                    return response;
                }

                bool duplicate = await _unitOfWork.UserRepository.IsExists(d =>
                    (d.UserName == request.UserName || d.FullName == request.FullName) && d.Id != userId);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Tên người dùng '{request.FullName} ({request.UserName})' đã tồn tại. Mời bạn xem lại.";
                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldUser = new User
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    ContactInfo = user.ContactInfo,
                    IsActive = user.IsActive
                };

                // Cập nhật thông tin người dùng
                user.UserName = request.UserName ?? user.UserName;
                user.FullName = request.FullName ?? user.FullName;
                user.ContactInfo = request.ContactInfo ?? user.ContactInfo;
                user.IsActive = request.IsActive;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Message = $"Lỗi khi cập nhật thông tin: {string.Join(", ", result.Errors.Select(e => e.Description))}";
                    return response;
                }

                // Cập nhật vai trò nếu có thay đổi
                var currentRoles = await _userManager.GetRolesAsync(user);
                var newRoles = request.Roles ?? new List<string>();

                if (!currentRoles.SequenceEqual(newRoles))
                {
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        response.Success = false;
                        response.Message = $"Lỗi khi xóa vai trò cũ: {string.Join(", ", removeRolesResult.Errors.Select(e => e.Description))}";
                        return response;
                    }

                    var addRolesResult = await _userManager.AddToRolesAsync(user, newRoles);
                    if (!addRolesResult.Succeeded)
                    {
                        response.Success = false;
                        response.Message = $"Lỗi khi thêm vai trò mới: {string.Join(", ", addRolesResult.Errors.Select(e => e.Description))}";
                        return response;
                    }
                }

                // Ghi log thay đổi
                var saveLog = await _changeLogService.Add(oldUser, user, nameof(User), userId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                var updatedRoles = await _userManager.GetRolesAsync(user);
                response.Data = user.ConvertToUserDto(updatedRoles.ToList());
                response.Success = true;
                response.Message = "Cập nhật thông tin người dùng thành công.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình cập nhật người dùng: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<UserDto>>> GetAvailableUsersForShift()
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();

            try
            {
                // 1️⃣ Lấy danh sách Role có tên "user"
                var userRole = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (userRole == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy vai trò 'user'.";
                    return response;
                }

                // 2️⃣ Lấy danh sách UserId có Role = "user"
                var userIdsWithUserRole = await _unitOfWork.UserRoleRepository.GetAllAsync(ur => ur.RoleId == userRole.Id);
                var userIds = userIdsWithUserRole.Select(ur => ur.UserId).ToHashSet();

                // 3️⃣ Lọc danh sách users theo danh sách UserId ở bước trên
                var users = await _unitOfWork.UserRepository.GetAllAsync(u => userIds.Contains(u.Id));

                // 4️⃣ Lấy toàn bộ UserRoles để gán vào Dictionary
                var userRolesList = await _unitOfWork.UserRoleRepository.GetAllAsync();
                var roles = await _roleManager.Roles.ToListAsync();
                var roleDict = roles.ToDictionary(r => r.Id, r => r.Name ?? "Unknown");

                var userRolesDict = userRolesList
                    .GroupBy(ur => ur.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(ur => roleDict.GetValueOrDefault(ur.RoleId, "Unknown")).ToList()
                    );

                // 5️⃣ Convert User -> UserDto
                var userDtos = users.ConvertToUserDto(userRolesDict);

                response.Success = true;
                response.Data = userDtos;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình lấy danh sách người dùng trong ca trực: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> ToggleActiveAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return CreateErrorResponse("Không tìm thấy người dùng.");

                user.IsActive = !user.IsActive; // Đảo trạng thái IsActive
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                    return CreateErrorResponse("Không thể cập nhật trạng thái người dùng.");

                return new ServiceResponse<bool>
                {
                    Success = true,
                    Data = user.IsActive,
                    Message = $"Đã {(user.IsActive ? "bật" : "tắt")} trạng thái hoạt động của người dùng."
                };
            }
            catch (Exception ex)
            {
                return CreateErrorResponse($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }


        #endregion
    }
}
