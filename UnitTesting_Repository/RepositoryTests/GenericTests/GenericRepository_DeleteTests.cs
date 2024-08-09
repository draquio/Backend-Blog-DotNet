using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;


namespace UnitTesting_Repository.RepositoryTests.GenericTests
{
    public class GenericRepository_DeleteTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;
        User deleteUser = new User { Id = 1, Username = "User1", Email = "user1@mail.com", Password = "password1" };
        public GenericRepository_DeleteTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private async Task SeedDatabase(BlogDBContext context)
        {
            context.Set<User>().AddRange(
                new User { Id = 1, Username = "User1", Email = "user1@mail.com", Password = "password1" },
                new User { Id = 2, Username = "User2", Email = "user2@mail.com", Password = "password2" }
            );
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task Delete_ShouldRemoveItem_WhenModelIsValid()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<User>(context);
                var result = await repository.Delete(deleteUser);

                Assert.True(result);
                var user = await context.Set<User>().FindAsync(1);
                Assert.Null(user);
            }
        }
        [Fact]
        public async Task Delete_ShouldThrowException_WhenAnErrorOccurs()
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
                var exception = await Assert.ThrowsAsync<Exception>(() => repository.Delete(deleteUser));
                Assert.Equal("Simulated exception", exception.Message);
            }
        }
    }
}
