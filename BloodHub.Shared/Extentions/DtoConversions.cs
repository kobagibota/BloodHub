using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;
using System.Text.Json;

namespace BloodHub.Shared.Extentions
{
    public static class DtoConversions
    {
        #region Auth

        public static AuthDto ConvertToAuthDto(this User user, IEnumerable<string>? roles)
        {
            return new AuthDto
            {
                Id = user.Id,
                Username = user.UserName!,
                FullName = user.FullName,
                Roles = string.Join(", ", roles ?? Enumerable.Empty<string>()) // Tránh lỗi null
            };
        }

        public static IEnumerable<AuthDto> ConvertToAuthDto(this IEnumerable<User> users, Dictionary<int, IEnumerable<string>> userRoles)
        {
            return users.Select(user =>
                user.ConvertToAuthDto(userRoles.TryGetValue(user.Id, out var roles) ? roles : Enumerable.Empty<string>())
            );
        }

        #endregion

        #region User

        public static UserDto ConvertToUserDto(this User user, List<string> roles)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                FullName = user.FullName,
                ContactInfo = user.ContactInfo,
                IsActive = user.IsActive,
                Roles = roles
            };
        }

        public static IEnumerable<UserDto> ConvertToUserDto(this IEnumerable<User> users, Dictionary<int, List<string>> userRolesDict)
        {
            return users.Select(user =>
                user.ConvertToUserDto(userRolesDict.GetValueOrDefault(user.Id, new List<string>()) ?? new List<string>())
            ) ?? Enumerable.Empty<UserDto>();
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

        #region Shift

        public static ShiftDto ConvertToDto(this Shift shift, IEnumerable<ShiftUserDto> shiftUserDtos)
        {
            var shiftDto = new ShiftDto()
            {
                Id = shift.Id,
                ShiftName = shift.ShiftName ?? string.Empty,
                ShiftStart = shift.ShiftStart,
                ShiftEnd = shift.ShiftEnd,
                HandoverTime = shift.HandoverTime,
                ReceivedShiftId = shift.ReceivedShiftId ?? default(int),
                HandBy = shift.HandBy ?? default(int),
                ReceivedBy = shift.ReceivedBy ?? default(int),
                Note = shift.Note ?? string.Empty,
                Status = shift.Status,
                ShiftUsers = shiftUserDtos.ToList()
            };
            return shiftDto;
        }


        public static IEnumerable<ShiftDto> ConvertToDto(this IEnumerable<Shift> shifts, IEnumerable<ShiftUserDto> shiftUserDtos)
        {
            return shifts.Select(s => s.ConvertToDto(shiftUserDtos));
        }

        #endregion

        #region ShiftDetail

        public static ShiftDetailDto ConvertToDto(this ShiftDetail shiftDetail)
        {
            var shiftDetailDto = new ShiftDetailDto()
            {
                Id = shiftDetail.Id,
                ShiftId = shiftDetail.ShiftId,
                ProductId = shiftDetail.ProductId,
                ProductName = shiftDetail.Product.ProductName,
                BloodGroup = shiftDetail.BloodGroup,
                Rhesus = shiftDetail.Rhesus,
                Volume = shiftDetail.Volume,
                StartingQuantity = shiftDetail.StartingQuantity,
                ImportedQuantity = shiftDetail.ImportedQuantity,
                ReturnedQuantity = shiftDetail.ReturnedQuantity,
                ExportedQuantity = shiftDetail.ExportedQuantity,
                DestroyedQuantity = shiftDetail.DestroyedQuantity,
                EndingQuantity = shiftDetail.EndingQuantity
            };
            return shiftDetailDto;
        }


        public static IEnumerable<ShiftDetailDto> ConvertToDto(this IEnumerable<ShiftDetail> shiftDetails)
        {
            return shiftDetails.Select(s => s.ConvertToDto());
        }

        #endregion

        #region ChangeLog

        #endregion
    }
}
