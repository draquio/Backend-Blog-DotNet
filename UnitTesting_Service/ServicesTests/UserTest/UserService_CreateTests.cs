
using AutoMapper;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_CreateTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly UserService _userService;
        UserCreateDto userCreateDto = new UserCreateDto
        {
            Username = "TestUser",
            Password = "password123"
        };
        User user = new User
        {
            Id = 1,
            Username = "TestUser"
        };
        User userCreated = new User
        {
            Id = 1,
            Username = "TestUser"
        };
        UserReadDto userReadDto = new UserReadDto
        {
            Id = 1,
            Username = "TestUser"
        };
        public UserService_CreateTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _userService = new UserService(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockPasswordHasher.Object
            );
        }
        [Fact]
        public async Task Create_ShouldReturnUserReadDto_WhenUserIsCreatedSuccessfully()
        {
            _mockMapper.Setup(mapper => mapper.Map<User>(userCreateDto)).Returns(user);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, userCreateDto.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(repo => repo.Create(user)).ReturnsAsync(userCreated);
            _mockUserRepository.Setup(repo => repo.GetById(userCreated.Id)).ReturnsAsync(userCreated);
            _mockMapper.Setup(mapper => mapper.Map<UserReadDto>(userCreated)).Returns(userReadDto);
            var result = await _userService.Create(userCreateDto);

            Assert.NotNull(result);
            Assert.Equal(userReadDto.Id, result.Id);
            Assert.Equal(userReadDto.Username, result.Username);
        }
        [Fact]
        public async Task Create_ShouldThrowInvalidOperationException_WhenUserIsNotCreated()
        {
            _mockMapper.Setup(mapper => mapper.Map<User>(userCreateDto)).Returns(user);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, userCreateDto.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(repo => repo.Create(user)).ReturnsAsync((User)null);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.Create(userCreateDto));

            Assert.Equal("User couldn't be created", exception.Message);
        }

        [Fact]
        public async Task Create_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockMapper.Setup(mapper => mapper.Map<User>(userCreateDto)).Returns(user);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, userCreateDto.Password)).Returns("hashed_password");
            _mockUserRepository.Setup(repo => repo.Create(user)).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Create(userCreateDto));

            Assert.Equal("An error occurred while creating: Database error", exception.Message);
        }
        [Fact]
        public async Task Delete_ShouldThrowArgumentException_WhenIdIsInvalid()
        {
            int invalidId = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Delete(invalidId));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Id must be greater than or equal to 1.", exception.InnerException.Message);
        }
    }
}
