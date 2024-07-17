using BackendBlog.DTO.Category;


namespace BackendBlog.DTO.Post
{
    public class PostReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? Tags { get; set; }
        public List<CategoryListDto>? Categories { get; set; }
    }
}
