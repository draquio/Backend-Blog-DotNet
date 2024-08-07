using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Validators.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BackendBlog.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;

        public PostService(IPostRepository postRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _mapper = mapper;
        }

        public async Task<List<PostListDto>> GetPagedPosts(int page, int pageSize, bool? IsPublished = null)
        {
            try
            {
                PaginationValidator.ValidatePage(page);
                PaginationValidator.ValidatePageSize(pageSize);
                List<Post> posts = await _postRepository.GetPagedPosts(page, pageSize, IsPublished);
                if(posts == null)  return new List<PostListDto>();
                List<PostListDto> postListDtos = _mapper.Map<List<PostListDto>>(posts);
                return postListDtos;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of posts: {ex.Message}", ex);
            }
        }
        public async Task<PostReadDto> GetPostWithData(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                Post post = await _postRepository.GetPostWithData(id);
                if(post == null) throw new KeyNotFoundException($"Post with ID {id} not found");
                PostReadDto postReadDto = _mapper.Map<PostReadDto>(post);
                return postReadDto;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving: {ex.Message}", ex);
            }
        }
        public async Task<PostReadDto> Create(PostCreateDto postCreateDto)
        {
            try
            {
                List<int>? categories = postCreateDto.CategoryIds;
                Post post = _mapper.Map<Post>(postCreateDto);
                post.CreatedAt = DateTime.Now;
                Post postCreated = await _postRepository.Create(post, categories);
                if(postCreated == null || postCreated.Id == 0) throw new InvalidOperationException("Post couldn't be created");
                PostReadDto postReadDto = _mapper.Map<PostReadDto>(postCreated);
                return postReadDto;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while creating: {ex.Message}", ex);
            }
        }
        public async Task<bool> Update(PostUpdateDto postUpdateDto)
        {
            try
            {
                Post post = await _postRepository.GetById(postUpdateDto.Id);
                if(post == null) throw new KeyNotFoundException($"Post with ID {postUpdateDto.Id} not found");
                List<int>? categories = postUpdateDto.CategoryIds;
                _mapper.Map(postUpdateDto, post);
                post.UpdatedAt = DateTime.Now;
                bool response = await _postRepository.Update(post, categories);
                if (!response) throw new InvalidOperationException("Post couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating: {ex.Message}", ex);
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                Post post = await _postRepository.GetById(id);
                if(post == null) throw new KeyNotFoundException($"Post with ID {id} not found");
                bool response = await _postRepository.Delete(post);
                if (!response) throw new InvalidOperationException("Post couldn't be deleted");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting: {ex.Message}", ex);
            }
        }

        public async Task<List<PostListDto>> GetFilteredPosts(int? categoryId, int? userId, DateTime? startDate, DateTime? endDate, string? tag)
        {
            try
            {
                Expression<Func<Post, bool>> filter = post =>
                    (!categoryId.HasValue || post.PostCategories.Any(pc => pc.CategoryId == categoryId)) &&
                    (!userId.HasValue || post.UserId == userId) &&
                    (!startDate.HasValue || post.CreatedAt >= startDate) &&
                    (!endDate.HasValue || post.CreatedAt <= endDate) &&
                    (string.IsNullOrEmpty(tag) || post.Tags.Contains(tag));

                IQueryable<Post> posts = await _postRepository.GetPostsByFilter(filter);
                List<PostListDto> postListDto = _mapper.Map<List<PostListDto>>(await posts.ToListAsync());
                return postListDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the posts: {ex.Message}", ex);
            }
        }

        public async Task<List<PostListDto>> SearchPosts(string term)
        {
            try
            {
                List<Post> posts = await _postRepository.SearchPosts(term);
                if (posts == null) return new List<PostListDto>();
                List<PostListDto> postListDto = _mapper.Map<List<PostListDto>>(posts);
                return postListDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of posts: {ex.Message}", ex);
            }
        }
    }
}
