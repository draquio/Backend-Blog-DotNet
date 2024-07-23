namespace BackendBlog.DTO.Image
{
    public class ImageDetailDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? AltText { get; set; }
        public string CreatedAt { get; set; }
    }
}
