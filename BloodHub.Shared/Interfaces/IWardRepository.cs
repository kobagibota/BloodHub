using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface IWardRepository : IGenericRepository<Ward>
    {
        Task<bool> IsExists(int wardId, string wardName);
    }
}
