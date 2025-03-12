using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role?> GetByName(string roleName);

        Task<bool> IsExists(string roleName);
    }
}
