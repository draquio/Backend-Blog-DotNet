
using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using BackendBlog.Service.Interface;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_CreateTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;
        PostCreateDto postCreateDto = new PostCreateDto
        {
            Title = "Test Post",
            Content = "This is a test post.",
            CategoryIds = new List<int> { 1, 2 }
        };
        Post post = new Post
        {
            Title = "Test Post",
            Content = "This is a test post.",
            CreatedAt = DateTime.Now
        };
        Post postCreated = new Post
        {
            Id = 1,
            Title = "Test Post",
            Content = "This is a test post.",
            CreatedAt = DateTime.Now
        };
        PostReadDto postReadDto = new PostReadDto
        {
            Id = 1,
            Title = "Test Post",
            Content = "This is a test post."
        };
        public PostService_CreateTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task Create_ShouldReturnPostReadDto_WhenPostIsCreatedSuccessfully()
        {
            _mockMapper.Setup(mapper => mapper.Map<Post>(postCreateDto)).Returns(post);
            _mockPostRepository.Setup(repo => repo.Create(post, postCreateDto.CategoryIds)).ReturnsAsync(postCreated);
            _mockMapper.Setup(mapper => mapper.Map<PostReadDto>(postCreated)).Returns(postReadDto);
            var result = await _postService.Create(postCreateDto);

            Assert.NotNull(result);
            Assert.Equal(postReadDto.Id, result.Id);
            Assert.Equal(postReadDto.Title, result.Title);
        }
        [Fact]
        public async Task Create_ShouldThrowInvalidOperationException_WhenPostIsNotCreated()
        {
            _mockMapper.Setup(mapper => mapper.Map<Post>(postCreateDto)).Returns(post);
            _mockPostRepository.Setup(repo => repo.Create(post, postCreateDto.CategoryIds)).ReturnsAsync((Post)null);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _postService.Create(postCreateDto));

            Assert.Equal("Post couldn't be created", exception.Message);
        }
        [Fact]
        public async Task Create_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockMapper.Setup(mapper => mapper.Map<Post>(postCreateDto)).Returns(post);
            _mockPostRepository.Setup(repo => repo.Create(post, postCreateDto.CategoryIds)).ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.Create(postCreateDto));

            Assert.Equal("An error occurred while creating: Database error", exception.Message);
        }
    }
}
