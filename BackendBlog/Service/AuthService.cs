using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using Microsoft.AspNetCore.Identity;
namespace BackendBlog.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthService(IUserRepository userRepository, IMapper mapper, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> Login(LoginDto login)
        {
            try
            {
                User user = await _userRepository.GetByEmail(login.Email);
                if (user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password) != PasswordVerificationResult.Success)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
                return user;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
        }


        public async Task<UserListDto> Register(RegisterDto register)
        {
            try
            {
                User user = _mapper.Map<User>(register);
                user.Password = _passwordHasher.HashPassword(user, register.Password);
                user.RoleId = 3;
                User userCreated = await _userRepository.Create(user);
                if (userCreated == null || userCreated.Id == 0) throw new InvalidOperationException("Registration couldn't be completed correctly");
                User userSearched = await _userRepository.GetById(userCreated.Id);
                UserListDto userListDto = _mapper.Map<UserListDto>(userSearched);
                return userListDto;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while registering: {ex.Message}", ex);
            }
        }
    }
}
