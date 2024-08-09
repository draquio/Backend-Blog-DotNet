
using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;

namespace UnitTesting_Repository.RepositoryTests.GenericTests
{
    public class GenericRepository_GetTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;
        int page = 1, pageSize = 10;
        public GenericRepository_GetTests()
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
        public async Task GetAll_ShouldReturnAllItems_WhenItemsExist()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetAll(1, 10);

                Assert.NotNull(result);
                Assert.Equal(5, result.Count);
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnPagedItems_WhenPageSizeIsSet()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetAll(1, 2);

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Post 1", result[0].Title);
                Assert.Equal("Post 2", result[1].Title);
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoItemsExist()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetAll(1, 10);

                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetAll_ShouldReturnCorrectPage_WhenPageAndPageSizeAreSet()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new GenericRepository<Post>(context);

                var result = await repository.GetAll(2, 2);

                Assert.NotNull(result);
                Assert.Equal(2, result.Count);
                Assert.Equal("Post 3", result[0].Title);
                Assert.Equal("Post 4", result[1].Title);
            }
        }
    }
}
