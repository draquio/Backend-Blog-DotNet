using System.ComponentModel.DataAnnotations;

namespace BackendBlog.DTO.Comment
{
    public class CommentCreateDto
    {
        public string AuthorComment { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
