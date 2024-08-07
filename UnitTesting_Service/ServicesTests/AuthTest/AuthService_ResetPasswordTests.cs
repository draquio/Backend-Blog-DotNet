
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Service;
using Moq;
using BackendBlog.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace UnitTesting_Service.ServicesTests.AuthTest
{
    public class AuthService_ResetPasswordTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenVerifyRepository> _mockTokenVerifyRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly AuthService _authService;
        string validToken = "valid token";
        string invalidToken = "invalid token";
        string expiredToken = "valid token";
        TokenVerify validTokenVerify = new TokenVerify { Token = "valid token", ExpirationDate = DateTime.UtcNow.AddHours(1), UserId = 1 };
        TokenVerify expiredTokenVerify = new TokenVerify { Token = "invalid token", ExpirationDate = DateTime.UtcNow.AddHours(-1), UserId = 2 };
        User user = new User { Email = "Test@mail.com", Username = "Test", Password = "123", Id = 1, IsActive = true };
        public AuthService_ResetPasswordTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenVerifyRepository = new Mock<ITokenVerifyRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _authService = new AuthService(
                _mockUserRepository.Object,
                null,
                _mockPasswordHasher.Object,
                _mockTokenVerifyRepository.Object,
                _mockEmailService.Object
            );
        }
        [Fact]
        public async Task ResetPassword_ShouldReturnTrue_WhenTokenIsValid()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(validToken, TokenType.PasswordReset))
                                      .ReturnsAsync(validTokenVerify);
            _mockUserRepository.Setup(repo => repo.GetById(validTokenVerify.UserId))
                               .ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, It.IsAny<string>()))
                               .Returns("hashed_password");
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()))
                               .Returns(Task.FromResult(true));
            _mockTokenVerifyRepository.Setup(repo => repo.Delete(It.IsAny<TokenVerify>()))
                                      .Returns(Task.FromResult(true));
            _mockEmailService.Setup(service => service.SendNewPasswordEmail(It.IsAny<User>(), It.IsAny<string>()))
                             .Returns(Task.CompletedTask);

            var result = await _authService.ResetPassword(validToken);

            Assert.True(result);
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowSecurityTokenException_WhenTokenIsInvalid()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(invalidToken, TokenType.PasswordReset))
                                      .ReturnsAsync((TokenVerify)null);

            await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.ResetPassword(invalidToken));
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowSecurityTokenException_WhenTokenIsExpired()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(expiredToken, TokenType.PasswordReset))
                                      .ReturnsAsync(expiredTokenVerify);

            await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.ResetPassword(expiredToken));
        }

        [Fact]
        public async Task ResetPassword_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(validToken, TokenType.PasswordReset))
                                      .ReturnsAsync(validTokenVerify);
            _mockUserRepository.Setup(repo => repo.GetById(validTokenVerify.UserId))
                               .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.ResetPassword(validToken));
        }
    }
}
