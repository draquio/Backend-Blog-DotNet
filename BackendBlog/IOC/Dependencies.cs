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

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
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

            // Roles
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Administrador"));
                options.AddPolicy("RequireEditorRole", policy => policy.RequireRole("Editor"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("Usuario"));
            });

            // Repository
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenHistoryRepository, TokenHistoryRepository>();
            services.AddScoped<ICategoryRepository,  CategoryRepository>();
            services.AddScoped<IImageRepository,  ImageRepository>();
            services.AddScoped<IPostRepository,  PostRepository>();
            services.AddScoped<ITokenVerifyRepository,  TokenVerifyRepository>();
            services.AddScoped<ICommentRepository,  CommentRepository>();

            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenHistoryService, TokenHistoryService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICommentService, CommentService>();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));
            
            // Validations
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);

            // Swagger + JWT Bearer
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend Blog", Version = "v1" });

                // Add JWT authentication scheme
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyMethod();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
        }
    }
}
