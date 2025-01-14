using BloodHub.Shared.DTOs;
using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> Login(LoginDto loginDto);
    }
}
