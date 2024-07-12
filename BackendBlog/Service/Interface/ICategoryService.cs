using BackendBlog.DTO.Category;


namespace BackendBlog.Service.Interface
{
    public interface ICategoryService
    {
        Task<List<CategoryListDto>> GetPagedCategories(int page, int pageSize);
        Task<CategoryReadDto> GetById(int id);
        Task<CategoryReadDto> Create(CategoryCreateDto categoryCreateDto);
        Task<bool> Update(CategoryUpdateDto categoryUpdateDto);
        Task<bool> SoftDelete(int id);
    }
}
