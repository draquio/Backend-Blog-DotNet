using System.ComponentModel.DataAnnotations;

namespace BackendBlog.DTO.Comment
{
    public class CommentUpdateDto
    {
        public int Id { get; set; }
        public string AuthorComment { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
    }
}
