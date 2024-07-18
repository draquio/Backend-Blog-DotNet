using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Token;
using BackendBlog.DTO.User;
using BackendBlog.Model;

namespace BackendBlog.Service.Interface
{
    public interface IAuthService
    {
        Task<User> Login(LoginDto loginDto);
        Task<UserListDto> Register(RegisterDto registerDto);
        Task<bool> VerifyAccount(string token);
        Task<bool> RequestPasswordReset(ResetPasswordRequestDto requestReset);
        Task<bool> ResetPassword(string token);
        
    }
}
