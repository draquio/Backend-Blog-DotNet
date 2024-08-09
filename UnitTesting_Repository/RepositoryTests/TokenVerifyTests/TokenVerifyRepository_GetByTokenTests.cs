using BackendBlog.Context;
using BackendBlog.Model;
using BackendBlog.Repository;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace UnitTesting_Repository.RepositoryTests.TokenVerifyTests
{
    public class TokenVerifyRepository_GetByTokenTests
    {
        private readonly DbContextOptions<BlogDBContext> _dbContextOptions;

        public TokenVerifyRepository_GetByTokenTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<BlogDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }
        private async Task SeedDatabase(BlogDBContext context)
        {
            context.Set<TokenVerify>().RemoveRange(context.Set<TokenVerify>());
            await context.SaveChangesAsync();

            context.Set<TokenVerify>().AddRange(
                new TokenVerify { Id = 1, Token = "valid_token", Type = TokenType.EmailVerification, UserId = 1, ExpirationDate = DateTime.UtcNow.AddHours(1) },
                new TokenVerify { Id = 2, Token = "another_valid_token", Type = TokenType.PasswordReset, UserId = 2, ExpirationDate = DateTime.UtcNow.AddHours(1) }
            );
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByToken_ShouldReturnTokenVerify_WhenTokenExists()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                await SeedDatabase(context);
            }

            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new TokenVerifyRepository(context);

                var result = await repository.GetByToken("valid_token", TokenType.EmailVerification);

                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("valid_token", result.Token);
                Assert.Equal(TokenType.EmailVerification, result.Type);
            }
        }

        [Fact]
        public async Task GetByToken_ShouldReturnNull_WhenTokenDoesNotExist()
        {
            using (var context = new BlogDBContext(_dbContextOptions))
            {
                var repository = new TokenVerifyRepository(context);

                var result = await repository.GetByToken("nonexistent_token", TokenType.EmailVerification);

                Assert.Null(result);
            }
        }
    }
}
