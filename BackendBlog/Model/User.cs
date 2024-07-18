using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendBlog.Model
{
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = true;
        public int RoleId { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual Role? Role { get; set; }
        public virtual TokenVerify TokenVerifyEmail { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
        public virtual ICollection<TokenHistory> TokenHistories { get; set; } = new List<TokenHistory>();
    }
}
