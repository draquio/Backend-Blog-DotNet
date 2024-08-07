
using AutoMapper;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_GetByIdTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;
        int validId = 1;
        User user = new User
        {
            Id = 1,
            Username = "TestUser"
        };
        UserReadDto userReadDto = new UserReadDto
        {
            Id = 1,
            Username = "TestUser"
        };
        public UserService_GetByIdTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _userService = new UserService(
                _mockUserRepository.Object,
                _mockMapper.Object,
                null
            );
        }
        [Fact]
        public async Task GetById_ShouldReturnUserReadDto_WhenUserExists()
        {
            _mockUserRepository.Setup(repo => repo.GetById(validId))
                               .ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserReadDto>(user))
                       .Returns(userReadDto);

            var result = await _userService.GetById(validId);

            Assert.NotNull(result);
            Assert.Equal(validId, result.Id);
            Assert.Equal("TestUser", result.Username);
        }
        [Fact]
        public async Task GetById_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            _mockUserRepository.Setup(repo => repo.GetById(validId))
                               .ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetById(validId));

            Assert.Equal($"User with ID {validId} not found", exception.Message);
        }

        [Fact]
        public async Task GetById_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            int invalidId = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetById(invalidId));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Id must be greater than or equal to 1.", exception.InnerException.Message);
        }

        [Fact]
        public async Task GetById_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            int validId = 1;

            _mockUserRepository.Setup(repo => repo.GetById(validId))
                               .ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetById(validId));

            Assert.Equal("An error occurred while retrieving: Database error", exception.Message);
        }
    }
}
