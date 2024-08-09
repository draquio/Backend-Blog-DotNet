
using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTesting_Repository.RepositoryTests.GenericTests
{
    public class GenericRepository_CreateTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;
        User newUser = new User { Id = 1, Username = "User", Email = "test@mail.com", Password = "HashedPassword" };
        public GenericRepository_CreateTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        [Fact]
        public async Task Create_ShouldAddNewItem_WhenModelIsValid()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<User>(context);
                var result = await repository.Create(newUser);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("User", result.Username);
            }
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var user = await context.Set<User>().FindAsync(1);
                Assert.NotNull(user);
                Assert.Equal(1, user.Id);
                Assert.Equal("User", user.Username);
            }
        }
        [Fact]
        public async Task Create_ShouldThrowException_WhenAnErrorOccurs()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var mockContext = new Mock<BlogDBContext>(_dbContextOptions);
                mockContext.Setup(c => c.Set<User>()).Returns(context.Set<User>());
                mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Simulated exception"));
                var repository = new GenericRepository<User>(mockContext.Object);
                var exception = await Assert.ThrowsAsync<Exception>(() => repository.Create(newUser));

                Assert.Equal("Simulated exception", exception.Message);
            }
        }
    }
}
