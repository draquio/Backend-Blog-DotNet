using BackendBlog.Model;
using Microsoft.EntityFrameworkCore;

namespace BackendBlog.Context
{
    public class BlogDBContext : DbContext
    {
        public BlogDBContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<PostCategory> PostCategories { get; set; }
        public virtual DbSet<TokenHistory> TokenHistories { get; set; }
        public virtual DbSet<TokenVerify> TokenVerifies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostCategory
            modelBuilder.Entity<PostCategory>()
                .HasKey(postcategory => new { postcategory.PostId, postcategory.CategoryId });
            // One Post to Many PostCategories
            modelBuilder.Entity<PostCategory>()
                .HasOne(postcategory => postcategory.Post)
                .WithMany(post => post.PostCategories)
                .HasForeignKey(postcategory => postcategory.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            // One Category to Many PostCategories
            modelBuilder.Entity<PostCategory>()
                .HasOne(postcategory => postcategory.Category)
                .WithMany(category => category.PostCategories)
                .HasForeignKey(postcategory => postcategory.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Post - Comment
            // Many Comments to One Post
            modelBuilder.Entity<Post>()
                .HasMany(post => post.Comments)
                .WithOne(comment => comment.Post)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            // Post - User
            // Many post to One User
            modelBuilder.Entity<Post>()
                .HasOne(post => post.User)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Post - Image
            // Many post to one Image
            modelBuilder.Entity<Post>()
                .HasOne(post => post.Image)
                .WithMany(image => image.Posts)
                .HasForeignKey(post => post.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(user => user.Username).IsUnique();
                entity.HasIndex(user => user.Email).IsUnique();
            });

            // User - Role
            // Many Roles to One User
            modelBuilder.Entity<User>()
                .HasOne(user => user.Role)
                .WithMany(role => role.Users)
                .HasForeignKey(user => user.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .Property(user => user.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<User>()
                .Property(user => user.IsActive)
                .HasDefaultValue(true);

            // User - TokenVerifyEmail
            modelBuilder.Entity<User>()
                .HasOne(user => user.TokenVerifyEmail)
                .WithOne(token => token.User)
                .HasForeignKey<TokenVerify>(token => token.UserId);

            // Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Administrador" },
                new Role { Id = 2, Name = "Editor" },
                new Role { Id = 3, Name = "Usuario" }
            );

            //TokenHistory
            modelBuilder.Entity<TokenHistory>()
                .HasOne(tokenhistory => tokenhistory.User)
                .WithMany(user => user.TokenHistories)
                .HasForeignKey(tokenhistory => tokenhistory.UserId);
        }
    }
}
