using System.ComponentModel.DataAnnotations;

namespace BackendBlog.DTO.Category
{
    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
