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
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PostCategory> PostCategories { get; set; }
        public virtual DbSet<PostTag> PostTags { get; set; }
        public virtual DbSet<TokenHistory> TokenHistories { get; set; }
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

            // PostTag
            modelBuilder.Entity<PostTag>()
                .HasKey(posttag => new { posttag.PostId, posttag.TagId });
            // One Post to Many PostTags
            modelBuilder.Entity<PostTag>()
                .HasOne(posttag => posttag.Post)
                .WithMany(post => post.PostTags)
                .HasForeignKey(posttag => posttag.PostId)
                .OnDelete(DeleteBehavior.Restrict);
            // One Tag to Many PostTags
            modelBuilder.Entity<PostTag>()
                .HasOne(posttag => posttag.Tag)
                .WithMany(tag => tag.PostTags)
                .HasForeignKey(posttag => posttag.TagId)
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
