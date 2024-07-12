using BackendBlog.DTO.Auth;
using BackendBlog.DTO.User;

namespace BackendBlog.Service.Interface
{
    public interface IUserService
    {
        Task<List<UserListDto>> GetPagedUsers(int page, int pageSize);
        Task<UserReadDto> GetById(int id);
        Task<UserReadDto> Create(UserCreateDto userCreateDto);
        Task<bool> Update(UserUpdateDto userUpdateDto);
        Task<bool> Delete(int id);
    }
}
