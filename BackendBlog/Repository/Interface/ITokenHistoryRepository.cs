using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface ITokenHistoryRepository : IGenericRepository<TokenHistory>
    {
        Task<TokenHistory> GetByRefreshToken(string refreshToken);
    }
}
