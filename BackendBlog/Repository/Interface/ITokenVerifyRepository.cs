using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface ITokenVerifyRepository : IGenericRepository<TokenVerify>
    {
        Task<TokenVerify> GetByToken(string token, TokenType type);
    }
}
