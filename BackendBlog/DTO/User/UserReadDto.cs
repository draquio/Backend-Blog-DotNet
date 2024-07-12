

namespace BackendBlog.DTO.User
{
    public class UserReadDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string CreatedAt { get; set; }
    }
}
