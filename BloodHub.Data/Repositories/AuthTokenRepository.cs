using BloodHub.Data.Data;
using BloodHub.Shared.Entities;
using BloodHub.Shared.Extentions;
using BloodHub.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodHub.Data.Repositories
{
    public class AuthTokenRepository : GenericRepository<AuthToken>, IAuthTokenRepository
    {
        public AuthTokenRepository(AppDbContext context) : base(context)
        {
        }

        // Lấy token theo hash (so sánh bằng BCrypt)
        public async Task<AuthToken?> GetByTokenAsync(string tokenHash)
        {
            var tokens = await _dbContext.Set<AuthToken>().ToListAsync();
            return tokens.FirstOrDefault(t => BCrypt.Net.BCrypt.Verify(tokenHash, t.TokenHash));
        }

        // Lấy danh sách token của user theo loại token
        public async Task<IEnumerable<AuthToken>> GetTokensByUserIdAsync(int userId, TokenType tokenType)
        {
            return await _dbContext.Set<AuthToken>().Where(at => at.UserId == userId && at.TokenType == tokenType && at.IsActive).ToListAsync();
        }

        // Thu hồi tất cả token của user theo loại
        public async Task RevokeTokensByTypeAsync(int userId, TokenType tokenType)
        {
            var tokens = await GetTokensByUserIdAsync(userId, tokenType);
            foreach (var token in tokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.IsUsed = true;
            }

            _dbContext.UpdateRange(tokens);
            await _dbContext.SaveChangesAsync();
        }
    }
}