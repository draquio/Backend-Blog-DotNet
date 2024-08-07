
using AutoMapper;
using BackendBlog.DTO.Post;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_UpdateTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly PostService _postService;
        PostUpdateDto postUpdateDto = new PostUpdateDto
        {
            Id = 1,
            Title = "Updated Test Post",
            Content = "This is an updated test post.",
            CategoryIds = new List<int> { 1, 2 }
        };
        Post post = new Post
        {
            Id = 1,
            Title = "Original Test Post",
            Content = "This is the original test post."
        };
        public PostService_UpdateTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _mockMapper = new Mock<IMapper>();
            _postService = new PostService(
                _mockPostRepository.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task Update_ShouldReturnTrue_WhenPostIsUpdatedSuccessfully()
        {
            _mockPostRepository.Setup(repo => repo.GetById(postUpdateDto.Id)).ReturnsAsync(post);
            _mockMapper.Setup(mapper => mapper.Map(postUpdateDto, post));
            _mockPostRepository.Setup(repo => repo.Update(post, postUpdateDto.CategoryIds)).ReturnsAsync(true);

            var result = await _postService.Update(postUpdateDto);

            Assert.True(result);
        }
        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenPostNotFound()
        {
            _mockPostRepository.Setup(repo => repo.GetById(postUpdateDto.Id)).ReturnsAsync((Post)null);
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.Update(postUpdateDto));

            Assert.Equal($"Post with ID {postUpdateDto.Id} not found", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            _mockPostRepository.Setup(repo => repo.GetById(postUpdateDto.Id)).ReturnsAsync(post);
            _mockMapper.Setup(mapper => mapper.Map(postUpdateDto, post));
            _mockPostRepository.Setup(repo => repo.Update(post, postUpdateDto.CategoryIds)).ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _postService.Update(postUpdateDto));

            Assert.Equal("Post couldn't be updated", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockPostRepository.Setup(repo => repo.GetById(postUpdateDto.Id)).ReturnsAsync(post);
            _mockMapper.Setup(mapper => mapper.Map(postUpdateDto, post));
            _mockPostRepository.Setup(repo => repo.Update(post, postUpdateDto.CategoryIds)).ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.Update(postUpdateDto));

            Assert.Equal("An error occurred while updating: Database error", exception.Message);
        }
    }
}
