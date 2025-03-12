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

    public interface IUserService
    {
        Task<ServiceResponse<IEnumerable<UserDto>>> GetAll();
        Task<ServiceResponse<UserDto?>> GetById(int userId);
        Task<ServiceResponse<UserDto>> Add(UserRequest request);
        Task<ServiceResponse<UserDto>> Update(int userId, UserRequest request);
        Task<ServiceResponse<bool>> Delete(int userId);

        Task<ServiceResponse<UserDto?>> GetByUsername(string username);
        Task<ServiceResponse<IEnumerable<string>>> GetRoleByUserId(int userId);
        Task<ServiceResponse<bool>> AssignRoleToUser(int userId, string roleName);
        Task<ServiceResponse<bool>> RemoveRoleFromUser(int userId, string roleName);

        Task<ServiceResponse<IEnumerable<UserDto>>> GetAvailableUsersForShift();
        Task<ServiceResponse<bool>> ToggleActive(int userId);
    }

    #endregion

    public class UserService(IUnitOfWork unitOfWork, IChangeLogService changeLogService, IConfiguration configuration) : IUserService
    {
        #region Private properties

        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IChangeLogService _changeLogService = changeLogService;
        private readonly IConfiguration _configuration = configuration;

        #endregion

        #region Methods

        private string GenerateDefaultPassword(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("UserName không hợp lệ!");

            string adminSecret = _configuration["Security:DefaultUserPass"] ?? "@Blood";
            string firstLetterUpper = char.ToUpper(userName[0]) + userName.Substring(1);

            return $"{firstLetterUpper}{adminSecret}{userName.Length}";
        }

        public async Task<ServiceResponse<UserDto>> Add(UserRequest request)
        {
            var response = new ServiceResponse<UserDto>();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.FirstName))
                {
                    response.Success = false;
                    response.Message = "Bạn vui lòng nhập đầy đủ thông tin.";
                    return response;
                }

                // Kiểm tra tên người dùng đã tồn tại chưa
                var existingUser = await _unitOfWork.UserRepository.IsExists(u => u.Username == request.Username);
                if (existingUser == true)
                {
                    response.Success = false;
                    response.Message = $"Tên đăng nhập '{request.Username}' đã tồn tại. Mời bạn xem lại.";
                    return response;
                }

                // Tạo user mới
                var newUser = new User
                {
                    Username = request.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(GenerateDefaultPassword(request.Username), workFactor: 12),
                    Title = request.Title,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    ContactInfo = request.ContactInfo,
                    IsActive = request.IsActive,
                    IsOnDuty = request.IsOnDuty,
                    MustChangePassword = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Thêm user mới vào database
                await _unitOfWork.UserRepository.AddAsync(newUser);
                await _unitOfWork.SaveAsync();

                // Gán vai trò cho người dùng
                foreach (var role in request.Roles)
                {
                    await AssignRoleToUser(newUser.Id, role);
                }

                var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == newUser.Id, r => r.Role);
                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                response.Success = true;
                response.Data = newUser.ConvertToUserDto(roleNames);
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
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return CreateErrorResponse("Không tìm thấy người dùng.");
                }

                // Lưu thông tin trước khi xoá
                var oldUser = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    PasswordHash = user.PasswordHash,
                    Title = user.Title,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ContactInfo = user.ContactInfo,
                    IsActive = user.IsActive
                };

                // Xoá người dùng
                _unitOfWork.UserRepository.Remove(user);
                await _unitOfWork.SaveAsync();

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
                var users = await _unitOfWork.UserRepository.GetAsync(
                    include: q => q.Include(ur => ur.UserRoles).ThenInclude(r => r.Role));

                // Tạo dictionary chứa danh sách vai trò theo UserId
                var userRolesDict = users
                    .SelectMany(u => u.UserRoles, (user, userRole) => new { user.Id, userRole.Role.Name })
                    .GroupBy(ur => ur.Id)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(ur => ur.Name).ToList()
                    );

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

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Không tìm thấy người dùng.";
                return response;
            }

            var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == userId, r => r.Role);
            var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

            response.Success = true;
            response.Data = user.ConvertToUserDto(roleNames);
            return response;
        }

        public async Task<ServiceResponse<UserDto>> Update(int userId, UserRequest request)
        {
            var response = new ServiceResponse<UserDto>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy người dùng.";
                    return response;
                }

                bool duplicate = await _unitOfWork.UserRepository.IsExists(d =>
                    d.Username == request.Username && d.FirstName == request.FirstName && d.LastName == request.LastName && d.Id != userId);
                if (duplicate)
                {
                    response.Success = false;
                    response.Message = $"Người dùng '{request.Username}' đã tồn tại. Mời bạn xem lại.";
                    return response;
                }

                // Lưu thông tin trước khi thay đổi
                var oldUser = new User
                {
                    Id = user.Id,
                    Username = user.Username,
                    Title = user.Title,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ContactInfo = user.ContactInfo,
                    IsActive = user.IsActive,
                    IsOnDuty = user.IsOnDuty
                };

                // Cập nhật thông tin người dùng
                user.Title = request.Title ?? user.Title;
                user.FirstName = request.FirstName ?? user.FirstName;
                user.LastName = request.LastName ?? user.LastName;
                user.ContactInfo = request.ContactInfo ?? user.ContactInfo;
                user.IsActive = request.IsActive;
                user.IsOnDuty = request.IsOnDuty;

                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveAsync();

                // Cập nhật vai trò nếu có thay đổi
                var existingRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == userId, ur => ur.Role);

                var currentRoleNames = existingRoles.Where(ur => ur.Role != null).Select(ur => ur.Role.Name).ToHashSet();
                var newRoleNames = request.Roles.ToHashSet();

                // Xóa vai trò không còn tồn tại
                foreach (var role in currentRoleNames.Except(newRoleNames))
                {
                    await RemoveRoleFromUser(userId, role);
                }

                // Thêm vai trò mới
                foreach (var role in newRoleNames.Except(currentRoleNames))
                {
                    await AssignRoleToUser(userId, role);
                }

                // Ghi log thay đổi
                var saveLog = await _changeLogService.Add(oldUser, user, nameof(User), userId, ChangeType.Update);
                if (!saveLog.Success)
                {
                    response.Success = false;
                    response.Message = saveLog.Message;
                    return response;
                }

                var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == userId, r => r.Role);
                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                response.Data = user.ConvertToUserDto(roleNames);
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
                // 1️⃣ Lấy danh sách User có IsOnDuty = true và IsActive = true
                var usersOnDuty = await _unitOfWork.UserRepository.GetListByAsync(u => u.IsOnDuty && u.IsActive);

                // 2️⃣ Lấy toàn bộ UserRoles để tạo Dictionary
                var userIds = usersOnDuty.Select(u => u.Id).ToHashSet();
                var userRolesList = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => userIds.Contains(ur.UserId));

                var roles = await _unitOfWork.RoleRepository.GetAllAsync();
                var roleDict = roles.ToDictionary(r => r.Id, r => r.Name ?? "Unknown");

                var userRolesDict = userRolesList
                    .GroupBy(ur => ur.UserId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(ur => roleDict.GetValueOrDefault(ur.RoleId, "Unknown")).ToList()
                    );

                // 3️⃣ Convert User -> UserDto
                var userDtos = usersOnDuty.ConvertToUserDto(userRolesDict);

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

        public async Task<ServiceResponse<bool>> ToggleActive(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                    return CreateErrorResponse("Không tìm thấy người dùng.");

                user.IsActive = !user.IsActive; // Đảo trạng thái IsActive
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveAsync();

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

        public async Task<ServiceResponse<UserDto?>> GetByUsername(string username)
        {
            var response = new ServiceResponse<UserDto?>();

            try
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy người dùng.";
                    return response;
                }

                var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == user.Id, r => r.Role);
                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                response.Success = true;
                response.Data = user.ConvertToUserDto(roleNames);
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Xảy ra lỗi trong quá trình lấy thông tin người dùng.";
            }

            return response;
        }

        public async Task<ServiceResponse<IEnumerable<string>>> GetRoleByUserId(int userId)
        {
            var response = new ServiceResponse<IEnumerable<string>>();

            try
            {
                var userRoles = await _unitOfWork.UserRoleRepository.GetListByAsync(ur => ur.UserId == userId, r => r.Role);
                var roleNames = userRoles.Select(ur => ur.Role.Name).ToList();

                response.Success = true;
                response.Data = roleNames;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Xảy ra lỗi trong quá trình lấy danh sách vai trò cho người dùng: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> AssignRoleToUser(int userId, string roleName)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var role = await _unitOfWork.RoleRepository.GetByName(roleName);
                if (role == null)
                {
                    response.Success = false;
                    response.Message = $"Vai trò '{roleName}' không tồn tại.";
                    return response;
                }

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                };

                await _unitOfWork.UserRoleRepository.AddAsync(userRole);
                await _unitOfWork.SaveAsync();

                response.Success = true;
                response.Message = $"Đã gán vai trò '{roleName}' cho người dùng.";
            }
            catch (Exception ex)
            {
                response = CreateErrorResponse($"Xảy ra lỗi trong quá trình gán vai trò cho người dùng: {ex.Message}");
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> RemoveRoleFromUser(int userId, string roleName)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var role = await _unitOfWork.RoleRepository.GetByName(roleName);
                if (role == null)
                {
                    response.Success = false;
                    response.Message = $"Vai trò '{roleName}' không tồn tại.";
                    return response;
                }

                var userRole = await _unitOfWork.UserRoleRepository.GetFirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
                if (userRole != null)
                {
                    _unitOfWork.UserRoleRepository.Remove(userRole);
                    await _unitOfWork.SaveAsync();

                    response.Success = true;
                    response.Message = $"Đã xoá vai trò '{roleName}' cho người dùng.";
                }                
            }
            catch (Exception ex)
            {
                response = CreateErrorResponse($"Xảy ra lỗi trong quá trình xoá vai trò cho người dùng: {ex.Message}");
            }

            return response;
        }
                
        #endregion
    }
}
