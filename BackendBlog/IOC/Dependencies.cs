﻿using BackendBlog.Context;
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
using BackendBlog.Filter;
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

            //Password
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            //Repository
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenHistoryRepository, TokenHistoryRepository>();


            // Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenHistoryService, TokenHistoryService>();

            // AutoMapper
            services.AddAutoMapper(typeof(AutoMapperProfile));

            // Filter
            services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
        }
    }
}
