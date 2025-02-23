using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;

namespace BloodHub.Data.Repositories
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
    }
}
