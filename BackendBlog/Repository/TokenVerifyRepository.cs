using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class TokenVerifyRepository : GenericRepository<TokenVerify>, ITokenVerifyRepository
    {
        private readonly BlogDBContext _dbContext;

        public TokenVerifyRepository(BlogDBContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<TokenVerify> GetByToken(string token, TokenType type)
        {
            try
            {
                TokenVerify? tokenVerifyEmail = await _dbContext.Set<TokenVerify>()
                    .FirstOrDefaultAsync(t => t.Token == token && t.Type == type);
                return tokenVerifyEmail;
            }
            catch
            {
                throw;
            }
        }
    }
}
