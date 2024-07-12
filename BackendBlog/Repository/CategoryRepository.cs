using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly BlogDBContext _dbContext;

        public CategoryRepository(BlogDBContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> GetPagedCategories(int page, int pageSize)
        {
            try
            {
                List<Category> categories = await _dbContext.Set<Category>()
                    .Where(category => category.IsDelete == false)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return categories;
            }
            catch
            {
                throw;
            }
        }
    }
}
