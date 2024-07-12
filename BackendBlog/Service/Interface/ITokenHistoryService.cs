using BackendBlog.DTO.Token;
using BackendBlog.Model;

namespace BackendBlog.Service.Interface
{
    public interface ITokenHistoryService
    {
        Task<TokenReadDto> GenerateToken(User user);
        Task<TokenReadDto> RefreshToken(string refreshToken);
    }
}
