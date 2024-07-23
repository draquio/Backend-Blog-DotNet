using AutoMapper;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Validators.Pagination;
using Microsoft.AspNetCore.Identity;

namespace BackendBlog.Service
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<UserListDto>> GetPagedUsers(int page, int pageSize)
        {
            try
            {
                PaginationValidator.ValidatePage(page);
                PaginationValidator.ValidatePageSize(pageSize);

                List<User> users = await _userRepository.GetUsersWithRoles(page, pageSize);

                if(users == null) return new List<UserListDto>();
                List<UserListDto> userListDto = _mapper.Map<List<UserListDto>>(users);
                return userListDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Users: {ex.Message}", ex);
            }

        }
        public async Task<UserReadDto> GetById(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                User user = await _userRepository.GetById(id);
                if (user == null) throw new KeyNotFoundException($"User with ID {id} not found");
                UserReadDto userReadDto = _mapper.Map<UserReadDto>(user);
                return userReadDto;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving: {ex.Message}", ex);
            }
        }
        public async Task<UserReadDto> Create(UserCreateDto userCreateDto)
        {
            try
            {
                User user = _mapper.Map<User>(userCreateDto);
                user.Password = _passwordHasher.HashPassword(user, userCreateDto.Password);
                User userCreated = await _userRepository.Create(user);
                if(userCreated == null || userCreated.Id == 0) throw new InvalidOperationException("User couldn't be created");
                User userWithRol = await _userRepository.GetById(userCreated.Id);
                UserReadDto userReadDto = _mapper.Map<UserReadDto>(userWithRol);
                return userReadDto;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while creating: {ex.Message}", ex);
            }
        }

        public async Task<bool> Update(UserUpdateDto userUpdateDto)
        {
            try
            {
                User user = await _userRepository.GetById(userUpdateDto.Id);
                if (user == null) throw new KeyNotFoundException($"User with ID {userUpdateDto.Id} not found");
                _mapper.Map(userUpdateDto, user);
                user.Password = user.Password;
                bool response = await _userRepository.Update(user);
                if(!response) throw new InvalidOperationException("User couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updateing: {ex.Message}", ex);
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                User user = await _userRepository.GetById(id);
                if (user == null) throw new KeyNotFoundException($"User with ID {id} not found");
                bool response = await _userRepository.Delete(user);
                if(!response) throw new InvalidOperationException("User couldn't be deleted");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting: {ex.Message}", ex);
            }
        }

        public async Task<bool> ChangePassword(UserChangePasswordDto changePassword)
        {
            try
            {
                User user = await _userRepository.GetById(changePassword.Id);
                if (user == null) throw new KeyNotFoundException($"User with ID {changePassword.Id} not found");
                if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, changePassword.CurrentPassword) != PasswordVerificationResult.Success)
                {
                    throw new ApplicationException("Current password is incorrect");
                }
                user.Password = _passwordHasher.HashPassword(user, changePassword.NewPassword);
                bool response = await _userRepository.Update(user);
                if (!response) throw new InvalidOperationException("Password couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating password: {ex.Message}", ex);
            }
        }
    }
}
