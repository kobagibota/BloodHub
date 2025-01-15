using BloodHub.Shared.Entities;
using BloodHub.Shared.Request;

namespace BloodHub.Shared.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> IsExists(int productId, ProductRequest productRequest);
    }
}
