

using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace UnitTesting_Service.ServicesTests.AuthTest
{
    public class AuthService_VerifyAccountTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITokenVerifyRepository> _mockTokenVerifyRepository;
        private readonly AuthService _authService;
        string validToken = "valid_token";
        string invalidToken = "invalid_token";
        string expiredToken = "expired_token";
        TokenVerify validTokenVerify = new TokenVerify { Token = "valid token", ExpirationDate = DateTime.UtcNow.AddHours(1), UserId = 1 };
        TokenVerify expiredTokenVerify = new TokenVerify { Token = "invalid token", ExpirationDate = DateTime.UtcNow.AddHours(-1), UserId = 2 };
        User user = new User { Email = "Test@mail.com", Username = "Test", Password = "123", Id = 1, IsActive = true };
        public AuthService_VerifyAccountTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenVerifyRepository = new Mock<ITokenVerifyRepository>();
            _authService = new AuthService(
                _mockUserRepository.Object,
                null,
                null,
                _mockTokenVerifyRepository.Object,
                null
            );
        }
        [Fact]
        public async Task VerifyAccount_ShouldReturnTrue_WhenTokenIsValid()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(validToken, TokenType.EmailVerification))
                                      .ReturnsAsync(validTokenVerify);
            _mockUserRepository.Setup(repo => repo.GetById(validTokenVerify.UserId))
                               .ReturnsAsync(user);
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>()))
                               .Returns(Task.FromResult(true));
            _mockTokenVerifyRepository.Setup(repo => repo.Delete(It.IsAny<TokenVerify>()))
                                      .Returns(Task.FromResult(true));

            var result = await _authService.VerifyAccount(validToken);

            Assert.True(result);
        }
        [Fact]
        public async Task VerifyAccount_ShouldThrowSecurityTokenException_WhenTokenIsInvalid()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(invalidToken, TokenType.EmailVerification))
                                      .ReturnsAsync((TokenVerify)null);

            await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.VerifyAccount(invalidToken));
        }

        [Fact]
        public async Task VerifyAccount_ShouldThrowSecurityTokenException_WhenTokenIsExpired()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(expiredToken, TokenType.EmailVerification))
                                      .ReturnsAsync(expiredTokenVerify);

            await Assert.ThrowsAsync<SecurityTokenException>(() => _authService.VerifyAccount(expiredToken));
        }

        [Fact]
        public async Task VerifyAccount_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            _mockTokenVerifyRepository.Setup(repo => repo.GetByToken(validToken, TokenType.EmailVerification))
                                      .ReturnsAsync(validTokenVerify);
            _mockUserRepository.Setup(repo => repo.GetById(validTokenVerify.UserId))
                               .ReturnsAsync((User)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _authService.VerifyAccount(validToken));
        }
    }
}
