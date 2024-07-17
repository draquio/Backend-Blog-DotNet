using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        private readonly BlogDBContext _dbContext;

        public ImageRepository(BlogDBContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<List<Image>> GetPagedImages(int page, int pageSize)
        {
            try
            {
                List<Image> images = await _dbContext.Set<Image>()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return images;
            }
            catch
            {
                throw;
            }
        }
    }
}
