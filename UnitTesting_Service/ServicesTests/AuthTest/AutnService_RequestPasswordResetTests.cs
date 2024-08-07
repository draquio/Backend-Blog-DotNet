
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Service;
using Moq;
using BackendBlog.DTO.Auth;

namespace UnitTesting_Service.ServicesTests.AuthTest
{
    public class AutnService_RequestPasswordResetTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenVerifyRepository> _mockTokenVerifyRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;
        ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto { Email = "Test@mail.com" };
        User user = new User { Email = "Test@mail.com", Username = "Test", Password = "123", Id = 1, IsActive = true };
        public AutnService_RequestPasswordResetTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenVerifyRepository = new Mock<ITokenVerifyRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _authService = new AuthService(
                _mockUserRepository.Object,
                null,
                null,
                _mockTokenVerifyRepository.Object,
                _mockEmailService.Object
                );
        }
        [Fact]
        public async Task RequestPasswordReset_ShouldReturnTrue_WhenEmailIsValid()
        {
            _mockUserRepository.Setup(repo => repo.GetByEmail(resetPasswordRequestDto.Email))
                               .ReturnsAsync(user);
            _mockTokenVerifyRepository.Setup(repo => repo.Create(It.IsAny<TokenVerify>()))
                                      .ReturnsAsync(new TokenVerify());
            _mockEmailService.Setup(service => service.SendPasswordResetEmail(It.IsAny<User>(), It.IsAny<string>()))
                             .Returns(Task.CompletedTask);
            var result = await _authService.RequestPasswordReset(resetPasswordRequestDto);

            Assert.True(result);
        }
        [Fact]
        public async Task RequestPasswordReset_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            ResetPasswordRequestDto resetPasswordRequestDto = new ResetPasswordRequestDto { Email = "nouser@mail.com" };
            _mockUserRepository.Setup(repo => repo.GetByEmail(resetPasswordRequestDto.Email))
                               .ReturnsAsync((User)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.RequestPasswordReset(resetPasswordRequestDto));
        }
    }
}
