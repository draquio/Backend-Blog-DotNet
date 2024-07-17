using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Category;
using BackendBlog.DTO.Image;
using BackendBlog.DTO.Post;
using BackendBlog.DTO.User;
using BackendBlog.Model;

namespace BackendBlog.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        { 
            // CreateMap<MapearDe, MapearA>
            #region User
            CreateMap<User, UserListDto>()
                .ForMember(user => user.RoleName, options => options.MapFrom(dto => dto.Role.Name));
            CreateMap<User, UserReadDto>()
                .ForMember(dto => dto.IsActive, options => options.MapFrom(user => user.IsActive == true ? 1 : 0))
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(user => user.CreatedAt.ToString("dd/MM/yyyy")))
                .ForMember(dto => dto.RoleName, options => options.MapFrom(user => user.Role.Name));
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            #endregion

            #region Auth
            CreateMap<RegisterDto, User>();
            #endregion

            #region Category
            CreateMap<Category, CategoryReadDto>()
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(category => category.CreatedAt.ToString("dd/MM/yyyy")))
                .ReverseMap();
            CreateMap<Category, CategoryListDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            #endregion

            #region Image
            CreateMap<Image, ImageDetailDto>()
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(image => image.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")));
            #endregion

            #region Post
            CreateMap<Post, PostListDto>()
                .ForMember(dto => dto.Username, options => options.MapFrom(post => post.User.Username))
                .ForMember(dto => dto.ImageUrl, options => options.MapFrom(post => post.Image.Url))
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(post => post.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")))
                .ForMember(dto => dto.Categories, options => options.MapFrom(post => post.PostCategories.Select(postcategory => postcategory.Category)));

            CreateMap<Post, PostCreateDto>().ReverseMap();
            CreateMap<Post, PostUpdateDto>().ReverseMap();
            CreateMap<Post, PostReadDto>()
                .ForMember(dto => dto.Username, options => options.MapFrom(post => post.User.Username))
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(post => post.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")))
                .ForMember(dto => dto.UpdatedAt, options => options.MapFrom(post => post.UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss")))
                .ForMember(dto => dto.ImageUrl, options => options.MapFrom(post => post.Image.Url))
                .ForMember(dto => dto.Categories, options => options.MapFrom(post => post.PostCategories.Select(postcategory => postcategory.Category)));
            #endregion
        }
    }
}
