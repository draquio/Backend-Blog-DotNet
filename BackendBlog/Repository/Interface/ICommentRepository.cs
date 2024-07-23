using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<List<Comment>> GetPagedComments(int page, int pageSize);
        Task<List<Comment>> GetPagedCommentsByPostId(int id, int page, int pageSize, bool? isApproved);
    }
}
