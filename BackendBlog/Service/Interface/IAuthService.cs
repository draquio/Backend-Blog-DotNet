using BackendBlog.DTO.Auth;
using BackendBlog.DTO.User;
using BackendBlog.Model;

namespace BackendBlog.Service.Interface
{
    public interface IAuthService
    {
        Task<User> Login(LoginDto loginDto);
        Task<UserListDto> Register(RegisterDto registerDto);
        
    }
}
