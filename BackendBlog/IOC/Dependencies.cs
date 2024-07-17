using BackendBlog.Context;
using BackendBlog.Repository.Interface;
using BackendBlog.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackendBlog.Service;
using BackendBlog.Service.Interface;
using BackendBlog.Mapper;
using BackendBlog.Model;
using Microsoft.AspNetCore.Identity;
using BackendBlog.Validators.Category;
using BackendBlog.Validators.Comment;
using BackendBlog.Validators.Role;
using FluentValidation;
using FluentValidation.AspNetCore;
using BackendBlog.Validators.Auth;
using BackendBlog.Validators.Post;
using BackendBlog.Validators.User;
namespace BackendBlog.IOC
{
    public static class Dependencies
    {
        public static void InjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Db
            services.AddDbContext<BlogDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Connection"));
            });
            
            // JWT
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Password
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Repository
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenHistoryRepository, TokenHistoryRepository>();
            services.AddScoped<ICategoryRepository,  CategoryRepository>();
            services.AddScoped<IImageRepository,  ImageRepository>();
            services.AddScoped<IPostRepository,  PostRepository>();

            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenHistoryService, TokenHistoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPostService, PostService>();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));
            
            // Validations
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

        }
    }
}
