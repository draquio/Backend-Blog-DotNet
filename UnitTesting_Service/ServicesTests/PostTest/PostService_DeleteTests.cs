
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.PostTest
{
    public class PostService_DeleteTests
    {
        private readonly Mock<IPostRepository> _mockPostRepository;
        private readonly PostService _postService;
        int validId = 1;
        Post post = new Post
        {
            Id = 1,
            Title = "Test Post"
        };
        public PostService_DeleteTests()
        {
            _mockPostRepository = new Mock<IPostRepository>();
            _postService = new PostService(
                _mockPostRepository.Object,
                null
            );
        }
        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenPostIsDeletedSuccessfully()
        {
            _mockPostRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Delete(post)).ReturnsAsync(true);
            var result = await _postService.Delete(validId);

            Assert.True(result);
        }
        [Fact]
        public async Task Delete_ShouldThrowKeyNotFoundException_WhenPostNotFound()
        {
            _mockPostRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync((Post)null);
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.Delete(validId));

            Assert.Equal($"Post with ID {validId} not found", exception.Message);
        }

        [Fact]
        public async Task Delete_ShouldThrowInvalidOperationException_WhenDeleteFails()
        {
            _mockPostRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Delete(post)).ReturnsAsync(false);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _postService.Delete(validId));

            Assert.Equal("Post couldn't be deleted", exception.Message);
        }

        [Fact]
        public async Task Delete_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            int invalidId = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.Delete(invalidId));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Id must be greater than or equal to 1.", exception.InnerException.Message);
        }

        [Fact]
        public async Task Delete_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockPostRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(post);
            _mockPostRepository.Setup(repo => repo.Delete(post)).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _postService.Delete(validId));

            Assert.Equal("An error occurred while deleting: Database error", exception.Message);
        }
    }
}
