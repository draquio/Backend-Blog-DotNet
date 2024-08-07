
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_DeleteTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;
        int validId = 1;
        User user = new User
        {
            Id = 1,
            Username = "TestUser"
        };
        public UserService_DeleteTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(
                _mockUserRepository.Object,
                null,
                null
            );
        }
        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenUserIsDeletedSuccessfully()
        {
            _mockUserRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Delete(user)).ReturnsAsync(true);
            var result = await _userService.Delete(validId);

            Assert.True(result);
        }
        [Fact]
        public async Task Delete_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            int validId = 1;

            _mockUserRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.Delete(validId));

            Assert.Equal($"User with ID {validId} not found", exception.Message);
        }

        [Fact]
        public async Task Delete_ShouldThrowInvalidOperationException_WhenDeleteFails()
        {
            _mockUserRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Delete(user)).ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.Delete(validId));

            Assert.Equal("User couldn't be deleted", exception.Message);
        }

        [Fact]
        public async Task Delete_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockUserRepository.Setup(repo => repo.GetById(validId)).ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Delete(user)).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Delete(validId));

            Assert.Equal("An error occurred while deleting: Database error", exception.Message);
        }
    }
}
