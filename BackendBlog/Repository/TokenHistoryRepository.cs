using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class TokenHistoryRepository : GenericRepository<TokenHistory>, ITokenHistoryRepository
    {
        private readonly BlogDBContext _dbContext;

        public TokenHistoryRepository(BlogDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TokenHistory> GetByRefreshToken(string refreshToken)
        {
            try
            {
                TokenHistory? tokenHistory = await _dbContext.Set<TokenHistory>()
                    .Include(token => token.User).ThenInclude(user => user.Role).AsNoTracking()
                    .FirstOrDefaultAsync(token => token.RefreshToken == refreshToken);
                return tokenHistory;
            }
            catch
            {
                throw;
            }
        }
    }
}
