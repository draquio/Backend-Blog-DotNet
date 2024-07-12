using System.ComponentModel.DataAnnotations;

namespace BackendBlog.DTO.User
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public int RoleId { get; set; }
    }
}
