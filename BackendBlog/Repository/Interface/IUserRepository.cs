using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<List<User>> GetUsersWithRoles(int page, int pageSize);
        Task<User> GetByEmail(string email);
    }
}
