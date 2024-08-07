using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using BackendBlog.Service.Interface;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTesting_Service.ServicesTests.AuthTest
{
    public class AuthService_LoginTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly Mock<ITokenVerifyRepository> _mockTokenVerifyRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        User user = new User { Username = "Test", Email = "Test@mail.com", Password = "hashedPassword", Id = 1 };
        LoginDto loginDto = new LoginDto { Email = "Test@mail.com", Password = "123" };
        public AuthService_LoginTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenVerifyRepository = new Mock<ITokenVerifyRepository>();
            _mockEmailService = new Mock<IEmailService>();
            var config = new MapperConfiguration(cfg =>
            {
            });
            _mapper = config.CreateMapper();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _authService = new AuthService(
                _mockUserRepository.Object, 
                _mapper, _mockPasswordHasher.Object, 
                _mockTokenVerifyRepository.Object, 
                _mockEmailService.Object
                );
        }

        [Fact]
        public async Task Login_ShouldReturnUser_WhenCredentialsAreValid()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, loginDto.Password))
                               .Returns(PasswordVerificationResult.Success);
            var result = await _authService.Login(loginDto);

            Assert.NotNull(result);
            Assert.Equal(loginDto.Email, result.Email);
        }
        [Fact]
        public async Task Login_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, loginDto.Password))
                               .Returns(PasswordVerificationResult.Failed);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginDto));
        }

        [Fact]
        public async Task Login_ShouldThrowInvalidOperationException_WhenUserIsNotActive()
        {
            user.IsActive = false;
            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email))
                               .ReturnsAsync(user);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.Login(loginDto));
        }

        [Fact]
        public async Task Login_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(loginDto.Email))
                               .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(loginDto));
        }
    }
}
