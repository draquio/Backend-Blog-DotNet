using BackendBlog.DTO.Category;
using BackendBlog.DTO.Tag;
using BackendBlog.Model;

namespace BackendBlog.DTO.Post
{
    public class PostReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string CreatedAt { get; set; }
        public string? Image { get; set; }
        public List<CategoryListDto>? Categories { get; set; }
        public List<TagDetailsDto>? Tags { get; set; }
    }
}
