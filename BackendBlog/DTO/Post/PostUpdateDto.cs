namespace BackendBlog.DTO.Post
{
    public class PostUpdateDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int AuthorId { get; set; }
        public string? Image { get; set; }
        public int? Pubished {  get; set; }
        public List<int>? CategoryIds { get; set; }
        public List<int>? TagsIds { get; set; }
    }
}
