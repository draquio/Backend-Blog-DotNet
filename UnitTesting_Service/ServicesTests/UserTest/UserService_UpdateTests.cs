using AutoMapper;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_UpdateTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;
        UserUpdateDto userUpdateDto = new UserUpdateDto
        {
            Id = 1,
            Username = "UpdatedUser"
        };
        User user = new User
        {
            Id = 1,
            Username = "OldUser"
        };
        public UserService_UpdateTests()
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
        public async Task Update_ShouldReturnTrue_WhenUserIsUpdatedSuccessfully()
        {
            _mockUserRepository.Setup(repo => repo.GetById(userUpdateDto.Id)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(userUpdateDto, user));
            _mockUserRepository.Setup(repo => repo.Update(user)).ReturnsAsync(true);
            var result = await _userService.Update(userUpdateDto);

            Assert.True(result);
        }
        [Fact]
        public async Task Update_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            var userUpdateDto = new UserUpdateDto
            {
                Id = 1,
                Username = "UpdatedUser"
            };

            _mockUserRepository.Setup(repo => repo.GetById(userUpdateDto.Id)).ReturnsAsync((User)null);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.Update(userUpdateDto));

            Assert.Equal($"User with ID {userUpdateDto.Id} not found", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            _mockUserRepository.Setup(repo => repo.GetById(userUpdateDto.Id)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(userUpdateDto, user));
            _mockUserRepository.Setup(repo => repo.Update(user)).ReturnsAsync(false);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.Update(userUpdateDto));

            Assert.Equal("User couldn't be updated", exception.Message);
        }

        [Fact]
        public async Task Update_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockUserRepository.Setup(repo => repo.GetById(userUpdateDto.Id)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(userUpdateDto, user));
            _mockUserRepository.Setup(repo => repo.Update(user)).ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Update(userUpdateDto));

            Assert.Equal("An error occurred while updating: Database error", exception.Message);
        }
    }
}
