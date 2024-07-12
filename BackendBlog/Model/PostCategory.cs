using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackendBlog.Model
{
    public partial class PostCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int PostId { get; set; }
        public virtual Post? Post { get; set; }
        public virtual Category? Category { get; set; }
    }
}
