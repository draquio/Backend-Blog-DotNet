using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BackendBlog.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly BlogDBContext _dbContext;

        public PostRepository(BlogDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Post>> GetPagedPosts(int page, int pageSize, bool? IsPublished = null)
        {
            try
            {
                IQueryable<Post> query = _dbContext.Set<Post>()
                    .Include(post => post.PostCategories).ThenInclude(postcategory => postcategory.Category)
                    .Include(post => post.User)
                    .Include(post => post.Image)
                    .Include(post => post.Comments);
                if (IsPublished.HasValue)
                {
                    query = query.Where(post => post.Published == IsPublished);
                }
                List<Post>? posts = await query.Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                return posts;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Post> GetPostWithData(int id)
        {
            Post? post = await _dbContext.Set<Post>()
                .Include(post => post.PostCategories).ThenInclude(postcategory => postcategory.Category)
                .Include(post => post.User)
                .Include(post => post.Image)
                .Include(post => post.Comments)
                .FirstOrDefaultAsync(post => post.Id == id);
            return post;
        }
        public async Task<Post> Create(Post post, List<int> categoriesIds)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await Create(post);
                if(categoriesIds != null && categoriesIds.Any())
                {
                    foreach (int categoryId in categoriesIds)
                    {
                        PostCategory categoryPost = new PostCategory()
                        {
                            CategoryId = categoryId,
                            PostId = post.Id,
                        };
                        _dbContext.Set<PostCategory>().Add(categoryPost);
                    }
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                Post newPost = await GetPostWithData(post.Id);
                return newPost;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> Update(Post post, List<int> categoriesIds)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                await Update(post);
                if (categoriesIds != null && categoriesIds.Any())
                {
                    List<PostCategory> existingCategories = await _dbContext.Set<PostCategory>()
                        .Where(postcategory => postcategory.PostId == post.Id)
                        .ToListAsync();

                    List<PostCategory> categoriesToRemove = existingCategories
                        .Where(existingCategories => !categoriesIds.Contains(existingCategories.CategoryId))
                        .ToList();

                    List<PostCategory> categoriesToAdd = categoriesIds
                        .Where(categoryId => !existingCategories.Any(existingcategories => existingcategories.CategoryId == categoryId))
                        .Select(categoryId => new PostCategory
                        {
                            CategoryId = categoryId,
                            PostId = post.Id,
                        })
                        .ToList();

                    _dbContext.Set<PostCategory>().RemoveRange(categoriesToRemove);
                    _dbContext.Set<PostCategory>().AddRange(categoriesToAdd);
                }
                else
                {
                    List<PostCategory> existingCategories = await _dbContext.Set<PostCategory>()
                        .Where(postcategory => postcategory.PostId == post.Id)

                        .ToListAsync();
                    _dbContext.Set<PostCategory>().RemoveRange(existingCategories);
                }
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public new async Task<bool> Delete(Post post)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                List<PostCategory> existingCategories = await _dbContext.Set<PostCategory>()
                    .Where(postcategory => postcategory.PostId == post.Id)
                    .ToListAsync();

                if(existingCategories != null && existingCategories.Any())
                {
                    _dbContext.Set<PostCategory>().RemoveRange(existingCategories);
                }
                _dbContext.Set<Post>().Remove(post);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IQueryable<Post>> GetPostsByFilter(Expression<Func<Post, bool>> filter)
        {
            try
            {
                IQueryable<Post> query = _dbContext.Set<Post>()
                    .Include(post => post.PostCategories)
                        .ThenInclude(postcategory => postcategory.Category)
                    .Include(post => post.User)
                    .Include(post => post.Image);
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                return await Task.FromResult(query);
            }
            catch
            {

                throw;
            }
        }

        public async Task<List<Post>> SearchPosts(string term)
        {
            try
            {
                List<Post> posts = await _dbContext.Set<Post>()
                                .Include(post => post.PostCategories)
                                    .ThenInclude(postCategory => postCategory.Category)
                                .Include(post => post.User)
                                .Include(post => post.Image)
                                .Where(post => post.Title.Contains(term) || post.Content.Contains(term))
                                .ToListAsync();
                return posts;
            }
            catch
            {
                throw;
            }
        }
    }
}
