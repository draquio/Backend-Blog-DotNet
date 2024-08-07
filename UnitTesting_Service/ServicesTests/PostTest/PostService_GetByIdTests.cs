
using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_GetByIdTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;
        int validId = 1;
        Post post = new Post
        {
            Id = 1,
            Title = "Test Post"
        };
        PostReadDto postReadDto = new PostReadDto
        {
            Id = 1,
            Title = "Test Post"
        };
        public PostService_GetByIdTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task GetPostWithData_ShouldReturnPostReadDto_WhenPostExists()
        {
            _mockPostRepository.Setup(repo => repo.GetPostWithData(validId))
                               .ReturnsAsync(post);
            _mockMapper.Setup(mapper => mapper.Map<PostReadDto>(post))
                       .Returns(postReadDto);
            var result = await _postService.GetPostWithData(validId);

            Assert.NotNull(result);
            Assert.Equal(validId, result.Id);
            Assert.Equal("Test Post", result.Title);
        }
        [Fact]
        public async Task GetPostWithData_ShouldThrowKeyNotFoundException_WhenPostNotFound()
        {
            _mockPostRepository.Setup(repo => repo.GetPostWithData(validId))
                               .ReturnsAsync((Post)null);
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.GetPostWithData(validId));

            Assert.Equal($"Post with ID {validId} not found", exception.Message);
        }
        [Fact]
        public async Task GetPostWithData_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            int invalidId = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPostWithData(invalidId));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Id must be greater than or equal to 1.", exception.InnerException.Message);
        }

        [Fact]
        public async Task GetPostWithData_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockPostRepository.Setup(repo => repo.GetPostWithData(validId))
                               .ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.GetPostWithData(validId));

            Assert.Equal("An error occurred while retrieving: Database error", exception.Message);
        }
    }
}
