using BackendBlog.DTO.Post;
using BackendBlog.Model;
using System.Linq.Expressions;

namespace BackendBlog.Service.Interface
{
    public interface IPostService
    {
        Task<List<PostListDto>> GetPagedPosts(int page, int pageSize, bool? IsPublished = null);
        Task<PostReadDto> GetPostWithData(int id);
        Task<PostReadDto> Create(PostCreateDto postCreateDto);
        Task<bool> Update(PostUpdateDto postUpdateDto);
        Task<bool> Delete(int id);
        Task<List<PostListDto>> GetFilteredPosts(int? categoryId, int? userId, DateTime? startDate, DateTime? endDate, string tag);
        Task<List<PostListDto>> SearchPosts(string term);
    }
}
