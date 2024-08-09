
using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTesting_Repository.RepositoryTests.GenericTests
{
    public class GenericRepository_GetByIdTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;

        public GenericRepository_GetByIdTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private async Task SeedDatabase(BlogDBContext context)
        {
            context.Set<Post>().AddRange(
                new Post { Id = 1, Title = "Post 1" },
                new Post { Id = 2, Title = "Post 2" },
                new Post { Id = 3, Title = "Post 3" },
                new Post { Id = 4, Title = "Post 4" },
                new Post { Id = 5, Title = "Post 5" }
            );
            await context.SaveChangesAsync();
        }
        [Fact]
        public async Task GetById_ShouldReturnItem_WhenItemExists()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetById(1);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Post 1", result.Title);
            }
        }
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenItemDoesNotExist()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetById(999);

                Assert.Null(result);
            }
        }
    }
}
