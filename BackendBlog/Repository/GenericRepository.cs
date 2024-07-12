using BackendBlog.Context;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly BlogDBContext _dbContext;

        public GenericRepository(BlogDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<T>> GetAll(int page, int pageSize)
        {
            try
            {
                List<T> model = await _dbContext.Set<T>()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return model;
            }
            catch
            {
                throw;
            }
        }

        public async Task<T> GetById(int id)
        {
            try
            {
                T? model = await _dbContext.Set<T>().FindAsync(id);
                return model;
            }
            catch
            {
                throw;
            }
        }
        public async Task<T> Create(T model)
        {
            try
            {
                _dbContext.Set<T>().Add(model);
                await _dbContext.SaveChangesAsync();
                return model;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> Update(T model)
        {
            try
            {
                _dbContext.Set<T>().Update(model);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
        public async Task<bool> Delete(T model)
        {
            try
            {
                _dbContext.Set<T>().Remove(model);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

    }
}
