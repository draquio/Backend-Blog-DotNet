using BackendBlog.DTO.Category;
using BackendBlog.DTO.Tag;

namespace BackendBlog.DTO.Post
{
    public class PostListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int AuthorId { get; set; }
        public string? Image { get; set; }
        public List<CategoryListDto>? Categories { get; set; }
    }
}
