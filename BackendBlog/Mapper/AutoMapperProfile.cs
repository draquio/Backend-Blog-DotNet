using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.User;
using BackendBlog.Model;

namespace BackendBlog.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
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
        }
    }
}
