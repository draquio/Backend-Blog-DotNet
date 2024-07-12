using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<List<Category>> GetPagedCategories(int page, int pageSize);
    }
}
