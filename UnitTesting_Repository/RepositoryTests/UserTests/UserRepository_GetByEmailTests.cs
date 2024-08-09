
using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;

namespace UnitTesting_Repository.RepositoryTests.UserTests
{
    public class UserRepository_GetByEmailTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;

        public UserRepository_GetByEmailTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private async Task SeedDatabase(BlogDBContext context)
        {
            context.Set<User>().RemoveRange(context.Set<User>());
            await context.SaveChangesAsync();

            context.Set<User>().AddRange(
                new User { Id = 1, Username = "User1", Email = "user1@mail.com", Password = "password1", Role = new Role { Id = 1, Name = "Admin" } },
                new User { Id = 2, Username = "User2", Email = "user2@mail.com", Password = "password2", Role = new Role { Id = 2, Name = "User" } }
            );
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task GetByEmail_ShouldReturnUser_WhenUserExists()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByEmail("user1@mail.com");

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("User1", result.Username);
                Assert.Equal("Admin", result.Role.Name);
            }
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new UserRepository(context);
                var result = await repository.GetByEmail("nonexistent@mail.com");

                Assert.Null(result);
            }
        }
    }
}
