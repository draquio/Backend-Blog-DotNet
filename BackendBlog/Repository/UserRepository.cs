using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly BlogDBContext _dbContext;

        public UserRepository(BlogDBContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<List<User>> GetUsersWithRoles(int page, int pageSize)
        {
            try
            {
                List<User> users = await _dbContext.Set<User>()
                    .Include(user => user.Role)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return users;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public new async Task<User> GetById(int id)
        {
            try
            {
                User? user = await _dbContext.Set<User>()
                    .Include(user => user.Role)
                    .FirstOrDefaultAsync(user => user.Id == id);
                return user;
            }
            catch
            {
                throw;
            }
        }
        public async Task<User> GetByEmail(string email)
        {
            try
            {
                User? user = await _dbContext.Set<User>()
                    .Include(user => user.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);
                return user;
            }
            catch
            {
                throw;
            }
        }
    }
}
