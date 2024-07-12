using System.ComponentModel.DataAnnotations;

namespace BackendBlog.DTO.User
{
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public int RoleId { get; set; }
    }
}
