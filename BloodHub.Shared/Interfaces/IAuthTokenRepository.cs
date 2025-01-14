using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;

namespace BloodHub.Shared.Interfaces
{
    public interface IAuthTokenRepository : IGenericRepository<AuthToken>
    {
        Task<AuthToken?> GetByTokenAsync(string token);
        Task<IEnumerable<AuthToken>> GetTokensByUserIdAsync(int userId, TokenType tokenType); // Lấy token theo loại
        Task RevokeTokensByTypeAsync(int userId, TokenType tokenType); // Thu hồi token theo loại
    }
}
