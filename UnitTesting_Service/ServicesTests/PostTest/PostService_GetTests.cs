
using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_GetTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;
        int page = 1, pageSize = 10;
        bool? isPublished = true;
        List<Post> posts = new List<Post>
        {
            new Post { Id = 1, Title = "Test Post 1" },
            new Post { Id = 2, Title = "Test Post 2" }
        };
        List<PostListDto> postListDtos = new List<PostListDto>
        {
            new PostListDto { Id = 1, Title = "Test Post 1" },
            new PostListDto { Id = 2, Title = "Test Post 2" }
        };
        public PostService_GetTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task GetPagedPosts_ShouldReturnPostListDto_WhenPostsExistAndIsPublishedIsNull()
        {
            _mockPostRepository.Setup(repo => repo.GetPagedPosts(page, pageSize, null))
                               .ReturnsAsync(posts);
            _mockMapper.Setup(mapper => mapper.Map<List<PostListDto>>(posts))
                       .Returns(postListDtos);
            var result = await _postService.GetPagedPosts(page, pageSize, null);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Post 1", result[0].Title);
            Assert.Equal("Test Post 2", result[1].Title);
        }
        [Fact]
        public async Task GetPagedPosts_ShouldReturnPostListDto_WhenPostsExistAndIsPublishedIsTrue()
        {
            _mockPostRepository.Setup(repo => repo.GetPagedPosts(page, pageSize, isPublished))
                               .ReturnsAsync(posts);
            _mockMapper.Setup(mapper => mapper.Map<List<PostListDto>>(posts))
                       .Returns(postListDtos);

            var result = await _postService.GetPagedPosts(page, pageSize, isPublished);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Post 1", result[0].Title);
            Assert.Equal("Test Post 2", result[1].Title);
        }
        [Fact]
        public async Task GetPagedPosts_ShouldReturnPostListDto_WhenPostsExistAndIsPublishedIsFalse()
        {
            bool isPublished = false;
            _mockPostRepository.Setup(repo => repo.GetPagedPosts(page, pageSize, isPublished))
                               .ReturnsAsync(posts);
            _mockMapper.Setup(mapper => mapper.Map<List<PostListDto>>(posts))
                       .Returns(postListDtos);
            var result = await _postService.GetPagedPosts(page, pageSize, isPublished);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Post 1", result[0].Title);
            Assert.Equal("Test Post 2", result[1].Title);
        }

        [Fact]
        public async Task GetPagedPosts_ShouldReturnEmptyList_WhenNoPostsExist()
        {
            _mockPostRepository.Setup(repo => repo.GetPagedPosts(page, pageSize, isPublished))
                               .ReturnsAsync((List<Post>)null);
            var result = await _postService.GetPagedPosts(page, pageSize, isPublished);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPagedPosts_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockPostRepository.Setup(repo => repo.GetPagedPosts(page, pageSize, isPublished))
                               .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPagedPosts(page, pageSize, isPublished));

            Assert.Equal("An error occurred while retrieving the list of posts: Database error", exception.Message);
        }
        [Fact]
        public async Task GetPagedPosts_ShouldThrowArgumentException_WhenPageIsInvalid()
        {
            int invalidPage = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPagedPosts(invalidPage, pageSize, null));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page must be greater than or equal to 1.", exception.InnerException.Message);
        }
        [Fact]
        public async Task GetPagedPosts_ShouldThrowArgumentException_WhenPageSizeIsInvalid()
        {
            int invalidPageSize = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPagedPosts(page, invalidPageSize, null));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page size must be greater than or equal to 1.", exception.InnerException.Message);
        }
        [Fact]
        public async Task GetPagedPosts_ShouldThrowArgumentException_WhenPageSizeExceedsMax()
        {
            int invalidPageSize = 21;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPagedPosts(page, invalidPageSize, null));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page size must be less than or equal to 20.", exception.InnerException.Message);
        }
    }
}
