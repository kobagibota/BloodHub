using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Interfaces;
using BloodHub.Shared.Request;

namespace BloodHub.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> IsExists(int productId, ProductRequest productRequest)
        {
            return await ExistsAsync(d => d.ProductName == productRequest.ProductName && d.Unit == productRequest.Unit && d.Id != productId);
        }
    }
}
