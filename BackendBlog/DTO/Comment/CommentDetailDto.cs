namespace BackendBlog.DTO.Comment
{
    public class CommentDetailDto
    {
        public int Id { get; set; }
        public string AuthorComment { get; set; }
        public string PostTitle { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
    }
}
