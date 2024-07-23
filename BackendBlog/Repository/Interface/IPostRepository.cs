using BackendBlog.Model;
using System.Linq.Expressions;

namespace BackendBlog.Repository.Interface
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<List<Post>> GetPagedPosts(int page, int pageSize, bool? IsPublished = null);
        Task<Post> GetPostWithData(int id);
        Task<Post> Create(Post post, List<int> categoriesIds);
        Task<bool> Update(Post post, List<int> categoriesIds);
        new Task<bool> Delete(Post post);
        Task<IQueryable<Post>> GetPostsByFilter(Expression<Func<Post, bool>> filter);
        Task<List<Post>> SearchPosts(string term);
    }
}
