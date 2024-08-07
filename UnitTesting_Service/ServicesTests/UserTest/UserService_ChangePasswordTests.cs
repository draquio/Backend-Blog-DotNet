
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_ChangePasswordTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly UserService _userService;
        UserChangePasswordDto changePasswordDto = new UserChangePasswordDto
        {
            Id = 1,
            CurrentPassword = "currentPassword",
            NewPassword = "newPassword",
            RepeatPassword = "newPassword"
        };
        User user = new User
        {
            Id = 1,
            Password = "hashedCurrentPassword"
        };
        public UserService_ChangePasswordTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _userService = new UserService(
                _mockUserRepository.Object,
                null,
                _mockPasswordHasher.Object
            );
        }
        [Fact]
        public async Task ChangePassword_ShouldReturnTrue_WhenPasswordIsChangedSuccessfully()
        {
            _mockUserRepository.Setup(repo => repo.GetById(changePasswordDto.Id)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, changePasswordDto.CurrentPassword))
                               .Returns(PasswordVerificationResult.Success);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, changePasswordDto.NewPassword))
                               .Returns("hashedNewPassword");
            _mockUserRepository.Setup(repo => repo.Update(user)).ReturnsAsync(true);
            var result = await _userService.ChangePassword(changePasswordDto);

            Assert.True(result);
        }
        [Fact]
        public async Task ChangePassword_ShouldThrowKeyNotFoundException_WhenUserNotFound()
        {
            _mockUserRepository.Setup(repo => repo.GetById(changePasswordDto.Id)).ReturnsAsync((User)null);
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ChangePassword(changePasswordDto));

            Assert.Equal($"User with ID {changePasswordDto.Id} not found", exception.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldThrowApplicationException_WhenCurrentPasswordIsIncorrect()
        {
            _mockUserRepository.Setup(repo => repo.GetById(changePasswordDto.Id)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, changePasswordDto.CurrentPassword))
                               .Returns(PasswordVerificationResult.Failed);
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _userService.ChangePassword(changePasswordDto));

            Assert.Equal("Current password is incorrect", exception.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldThrowInvalidOperationException_WhenUpdateFails()
        {
            _mockUserRepository.Setup(repo => repo.GetById(changePasswordDto.Id)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, changePasswordDto.CurrentPassword))
                               .Returns(PasswordVerificationResult.Success);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, changePasswordDto.NewPassword))
                               .Returns("hashedNewPassword");
            _mockUserRepository.Setup(repo => repo.Update(user)).ReturnsAsync(false);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.ChangePassword(changePasswordDto));

            Assert.Equal("Password couldn't be updated", exception.Message);
        }

        [Fact]
        public async Task ChangePassword_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockUserRepository.Setup(repo => repo.GetById(changePasswordDto.Id)).ReturnsAsync(user);
            _mockPasswordHasher.Setup(hasher => hasher.VerifyHashedPassword(user, user.Password, changePasswordDto.CurrentPassword))
                               .Returns(PasswordVerificationResult.Success);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(user, changePasswordDto.NewPassword))
                               .Returns("hashedNewPassword");
            _mockUserRepository.Setup(repo => repo.Update(user)).ThrowsAsync(new Exception("Database error"));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.ChangePassword(changePasswordDto));

            Assert.Equal("An error occurred while updating password: Database error", exception.Message);
        }
    }
}
