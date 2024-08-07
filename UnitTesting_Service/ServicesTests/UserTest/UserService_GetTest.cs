
using AutoMapper;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service;
using Moq;

namespace UnitTesting_Service.ServicesTests.UserTest
{
    public class UserService_GetTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _userService;
        int page = 1, pageSize = 10;
        List<User> users = new List<User>
        {
            new User { Id = 1, Username = "TestUser1" },
            new User { Id = 2, Username = "TestUser2" }
        };
        List<UserListDto> userListDtos = new List<UserListDto>
        {
            new UserListDto { Id = 1, Username = "TestUser1" },
            new UserListDto { Id = 2, Username = "TestUser2" }
        };
        public UserService_GetTest()
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
        public async Task GetPagedUsers_ShouldReturnUserListDto_WhenUsersExist()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersWithRoles(page, pageSize))
                               .ReturnsAsync(users);
            _mockMapper.Setup(mapper => mapper.Map<List<UserListDto>>(users))
                       .Returns(userListDtos);
            var result = await _userService.GetPagedUsers(page, pageSize);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("TestUser1", result[0].Username);
            Assert.Equal("TestUser2", result[1].Username);
        }
        [Fact]
        public async Task GetPagedUsers_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersWithRoles(page, pageSize))
                               .ReturnsAsync((List<User>)null);
            var result = await _userService.GetPagedUsers(page, pageSize);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetPagedUsers_ShouldThrowApplicationException_WhenExceptionIsThrown()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersWithRoles(page, pageSize))
                               .ThrowsAsync(new Exception("Database error"));
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetPagedUsers(page, pageSize));

            Assert.Equal("An error occurred while retrieving the list of Users: Database error", exception.Message);
        }

        [Fact]
        public async Task GetPagedUsers_ShouldThrowArgumentException_WhenPageIsInvalid()
        {
            int invalidPage = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetPagedUsers(invalidPage, pageSize));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page must be greater than or equal to 1.", exception.InnerException.Message);
        }

        [Fact]
        public async Task GetPagedUsers_ShouldThrowArgumentException_WhenPageSizeIsInvalid()
        {
            int invalidPageSize = -1;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetPagedUsers(page, invalidPageSize));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page size must be greater than or equal to 1.", exception.InnerException.Message);
        }

        [Fact]
        public async Task GetPagedUsers_ShouldThrowArgumentException_WhenPageSizeExceedsMax()
        {
            int invalidPageSize = 21;
            var exception = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetPagedUsers(page, invalidPageSize));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("Page size must be less than or equal to 20.", exception.InnerException.Message);
        }
    }
}
