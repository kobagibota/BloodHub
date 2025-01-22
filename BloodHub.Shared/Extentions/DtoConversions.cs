using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using System.Text.Json;

namespace BloodHub.Shared.Extentions
{
    public static class DtoConversions
    {
        #region User

        public static UserDto ConvertToDto(this User user, IEnumerable<string> roles)
        {
            var userDto = new UserDto()
            {
                Id = user.Id,
                Username = user.UserName!,
                FullName = user.FullName,
                Roles = string.Join(", ", roles)
            };
            return userDto;
        }

        public static IEnumerable<UserDto> ConvertToDto(this IEnumerable<User> users, IEnumerable<string> roles)
        {
            return users.Select(user => user.ConvertToDto(roles));
        }

        #endregion

        #region ChangeLog

        public static ChangeLogDto ConvertToDto(this ChangeLog changeLog, string userName)
        {
            var changeDetails = JsonSerializer.Deserialize<List<ChangeLogDetail>>(changeLog.ChangeDetails!) ?? new List<ChangeLogDetail>();

            return new ChangeLogDto
            {
                Id = changeLog.Id,
                UserName = userName,
                ChangeType = changeLog.ChangeType,
                TableName = changeLog.TableName,
                RecordId = changeLog.RecordId,
                ChangeDetails = changeDetails,
                ChangeTimestamp = changeLog.ChangeTimestamp,
                IpAddress = changeLog.IpAddress,
                UserAgent = changeLog.UserAgent
            };
        }

        // Chuyển đổi một danh sách ChangeLog sang danh sách ChangeLogDto  
        public static IEnumerable<ChangeLogDto> ConvertToDto(this IEnumerable<ChangeLog> changeLogs, IEnumerable<User> users)
        {
            var userDictionary = users.ToDictionary(u => u.Id, u => u.UserName); // Tạo từ điển  

            return changeLogs.Select(cl => cl.ConvertToDto(userDictionary.GetValueOrDefault(cl.UserId) ?? "Unknown"));
        }

        #endregion

        #region Order

        #endregion

        #region ChangeLog

        #endregion
    }
}
