
using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_SearchTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;
        string searchTerm = "Test";
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
        public PostService_SearchTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task SearchPosts_ShouldReturnPostListDto_WhenPostsExist()
        {
            _mockPostRepository.Setup(repo => repo.SearchPosts(searchTerm))
                               .ReturnsAsync(posts);
            _mockMapper.Setup(mapper => mapper.Map<List<PostListDto>>(It.IsAny<List<Post>>()))
                       .Returns(postListDtos);
            var result = await _postService.SearchPosts(searchTerm);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Test Post 1", result[0].Title);
            Assert.Equal("Test Post 2", result[1].Title);
        }
        [Fact]
        public async Task SearchPosts_ShouldReturnEmptyList_WhenNoPostsFound()
        {
            _mockPostRepository.Setup(repo => repo.SearchPosts(searchTerm))
                               .ReturnsAsync((List<Post>)null);
            _mockMapper.Setup(mapper => mapper.Map<List<PostListDto>>(It.IsAny<List<Post>>()))
                       .Returns(new List<PostListDto>());
            var result = await _postService.SearchPosts(searchTerm);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
        [Fact]
        public async Task SearchPosts_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockPostRepository.Setup(repo => repo.SearchPosts(searchTerm))
                               .ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.SearchPosts(searchTerm));

            Assert.Equal("An error occurred while retrieving the list of posts: Database error", exception.Message);
        }
    }
}
