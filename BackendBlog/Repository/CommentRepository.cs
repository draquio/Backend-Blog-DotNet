using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Repository
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        private readonly BlogDBContext _dbContext;

        public CommentRepository(BlogDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Comment>> GetPagedComments(int page, int pageSize)
        {
            try
            {
                List<Comment>? comments = await _dbContext.Set<Comment>()
                                        .Include(comment => comment.Post)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
                return comments;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<Comment>> GetPagedCommentsByPostId(int id, int page, int pageSize, bool? isApproved)
        {
            try
            {
                IQueryable<Comment> query = _dbContext.Set<Comment>().Where(comment => comment.PostId == id);
                if (isApproved.HasValue)
                {
                    query = query.Where(comment => comment.IsApproved == isApproved);
                }
                List<Comment>? comments = await query.Include(comment => comment.Post)
                                                        .Skip((page - 1) * pageSize)
                                                        .Take(pageSize)
                                                        .ToListAsync();
                                            
                return comments;
            }
            catch
            {
                throw;
            }
        }
    }
}
