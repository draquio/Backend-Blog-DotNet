using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Service;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackendBlog.DTO.User;

namespace UnitTesting_Service.ServicesTests.AuthTest
{
    public class AuthService_RegisterTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly Mock<ITokenVerifyRepository> _mockTokenVerifyRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        RegisterDto registerDto = new RegisterDto { Email = "Test@mail.com", Username = "Test", Password = "123", RepeatPassword = "123" };
        UserListDto userDto = new UserListDto { Email = "Test@mail.com", Username = "Test", Id = 1, RoleId = 1 };
        User userCreated = new User { Email = "Test@mail.com", Username = "Test", Password = "123", Id = 1, IsActive = false };

        public AuthService_RegisterTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTokenVerifyRepository = new Mock<ITokenVerifyRepository>();
            _mockEmailService = new Mock<IEmailService>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RegisterDto, User>();
                cfg.CreateMap<User, UserListDto>();
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
        public async Task Register_ShouldReturnUser_WhenRegistrationIsSuccessful()
        {
            _mockUserRepository.Setup(repo => repo.Create(It.IsAny<User>()))
                               .ReturnsAsync(userCreated);
            _mockUserRepository.Setup(repo => repo.GetById(userCreated.Id))
                               .ReturnsAsync(userCreated);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<User>(), registerDto.Password))
                               .Returns("hashedPassword");
            _mockTokenVerifyRepository.Setup(repo => repo.Create(It.IsAny<TokenVerify>()))
                                      .ReturnsAsync(new TokenVerify());
            _mockEmailService.Setup(service => service.SendVerificationEmail(It.IsAny<User>(), It.IsAny<string>()))
                             .Returns(Task.CompletedTask);
            var result = await _authService.Register(registerDto);

            Assert.NotNull(result);
            Assert.Equal(registerDto.Email, result.Email);
        }

        [Fact]
        public async Task Register_ShouldThrowInvalidOperationException_WhenUserCreationFails()
        {
            _mockUserRepository.Setup(repo => repo.Create(It.IsAny<User>()))
                               .ReturnsAsync((User)null);
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<User>(), registerDto.Password))
                               .Returns("hashedPassword");

            await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.Register(registerDto));
        }

        [Fact]
        public async Task Register_ShouldThrowApplicationException_WhenAnErrorOccurs()
        {
            _mockUserRepository.Setup(repo => repo.Create(It.IsAny<User>()))
                               .ThrowsAsync(new Exception("Test Exception"));
            _mockPasswordHasher.Setup(hasher => hasher.HashPassword(It.IsAny<User>(), registerDto.Password))
                               .Returns("hashedPassword");

            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _authService.Register(registerDto));
            Assert.Equal("An error occurred while registering: Test Exception", exception.Message);
        }
    }
}
