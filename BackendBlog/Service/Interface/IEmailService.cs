using BackendBlog.Model;

namespace BackendBlog.Service.Interface
{
    public interface IEmailService
    {
        Task SendVerificationEmail(User user, string token);
        Task SendPasswordResetEmail(User user, string token);
        Task SendNewPasswordEmail(User user, string newPassword);
    }
}
