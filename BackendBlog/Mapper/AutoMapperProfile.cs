using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Category;
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

            CreateMap<RegisterDto, User>();

            CreateMap<UserCreateDto, User>();

            CreateMap<UserUpdateDto, User>();

            #endregion

            #region Category
            CreateMap<Category, CategoryReadDto>()
                .ForMember(dto => dto.CreatedAt, options => options.MapFrom(category => category.CreatedAt.ToString("dd/MM/yyyy")))
                .ReverseMap();
            CreateMap<Category, CategoryListDto>().ReverseMap();
            CreateMap<Category, CategoryUpdateDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();

            #endregion
        }
    }
}
