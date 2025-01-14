using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<string> GetFullNameById(int id);
    }
}
