using BloodHub.Shared.Entities;

namespace BloodHub.Shared.Interfaces
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        Task<bool> IsExists(int doctorId, string doctorName);
    }
}
