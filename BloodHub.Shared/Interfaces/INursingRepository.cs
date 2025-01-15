using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface INursingRepository : IGenericRepository<Nursing>
    {
        Task<bool> IsExists(int nursingId, string nursingName);
    }
}
