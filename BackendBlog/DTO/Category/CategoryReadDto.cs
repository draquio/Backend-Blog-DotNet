namespace BackendBlog.DTO.Category
{
    public class CategoryReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatedAt { get; set; }
    }
}
