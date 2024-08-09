using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
namespace UnitTesting_Repository.RepositoryTests.GenericTests
{
    public class GenericRepository_UpdateTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;
        User updateUser = new User { Id = 1, Username = "UpdatedUser", Email = "updateduser@mail.com", Password = "NewHashedPassword"};
        public GenericRepository_UpdateTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private async Task SeedDatabase(BlogDBContext context)
        {
            context.Set<User>().AddRange(
                new User { Id = 1, Username = "User1", Email = "test@mail.com", Password = "HashedPassword" },
                new User { Id = 2, Username = "User2", Email = "test@mail.com", Password = "HashedPassword" }
            );
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task Update_ShouldUpdateItem_WhenModelIsValid()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<User>(context);
                var result = await repository.Update(updateUser);

                Assert.True(result);

                var user = await context.Set<User>().FindAsync(1);
                Assert.NotNull(user);
                Assert.Equal("UpdatedUser", user.Username);
                Assert.Equal("updateduser@mail.com", user.Email);
                Assert.Equal("NewHashedPassword", user.Password);
            }
        }
        [Fact]
        public async Task Update_ShouldThrowException_WhenAnErrorOccurs()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }
            var mockContext = new Mock<BlogDBContext>(_dbContextOptions);
            using (var realContext = new BlogDBContext(_dbContextOptions))
            {
                mockContext.Setup(c => c.Set<User>()).Returns(realContext.Set<User>());
                mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Simulated exception"));
                var repository = new GenericRepository<User>(mockContext.Object);
                var exception = await Assert.ThrowsAsync<Exception>(() => repository.Update(updateUser));
                Assert.Equal("Simulated exception", exception.Message);
            }
        }
    }
}
