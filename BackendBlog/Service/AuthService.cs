using AutoMapper;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Token;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
namespace BackendBlog.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ITokenVerifyRepository _tokenVerifyRepository;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, 
            IMapper mapper, 
            IPasswordHasher<User> passwordHasher, 
            ITokenVerifyRepository tokenVerifyEmailRepository, 
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _tokenVerifyRepository = tokenVerifyEmailRepository;
            _emailService = emailService;
        }

        public async Task<User> Login(LoginDto login)
        {
            try
            {
                User user = await _userRepository.GetByEmail(login.Email);
                if(user.IsActive == false) throw new InvalidOperationException("Your account is not active. Please verify your email to activate your account.");
                if(user == null || _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password) != PasswordVerificationResult.Success)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
                return user;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (InvalidOperationException)
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
                user.IsActive = false;
                User userCreated = await _userRepository.Create(user);
                if (userCreated == null || userCreated.Id == 0) throw new InvalidOperationException("Registration couldn't be completed correctly");
                User userSearched = await _userRepository.GetById(userCreated.Id);
                UserListDto userListDto = _mapper.Map<UserListDto>(userSearched);

                // Token
                string token = GenerateVerificationToken();
                TokenVerify tokenVerify = new TokenVerify
                {
                    UserId = userCreated.Id,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    Type = TokenType.EmailVerification,
                    ExpirationDate = DateTime.UtcNow.AddDays(7)
                };
                await _tokenVerifyRepository.Create(tokenVerify);
                await _emailService.SendVerificationEmail(userCreated, token);

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

        public async Task<bool> RequestPasswordReset(ResetPasswordRequestDto requestReset)
        {
            try
            {
                GenericValidator.ValidateEmail(requestReset.Email);
                User user = await _userRepository.GetByEmail(requestReset.Email);
                if (user == null) throw new KeyNotFoundException($"Account with email {requestReset.Email} not found");
                if (user.IsActive == false) throw new InvalidOperationException("Your account is not active. Please verify your email to activate your account.");
                string token = GenerateVerificationToken();
                TokenVerify tokenVerify = new TokenVerify
                {
                    UserId = user.Id,
                    Token = token,
                    CreatedAt = DateTime.UtcNow,
                    Type = TokenType.PasswordReset,
                    ExpirationDate = DateTime.UtcNow.AddDays(7)
                };
                await _tokenVerifyRepository.Create(tokenVerify);
                await _emailService.SendPasswordResetEmail(user, token);
                return true;
            }
            catch (KeyNotFoundException) { throw; }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task<bool> ResetPassword(string token)
        {
            try
            {
                TokenVerify tokenVerify = await _tokenVerifyRepository.GetByToken(token, TokenType.PasswordReset);
                if(tokenVerify == null || tokenVerify.ExpirationDate < DateTime.UtcNow) throw new SecurityTokenException("Invalid token or expired");
                User user = await _userRepository.GetById(tokenVerify.UserId);
                if (user == null) throw new KeyNotFoundException($"User not found");
                string newPassword = GenerateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, newPassword);
                await _userRepository.Update(user);
                await _tokenVerifyRepository.Delete(tokenVerify);
                await _emailService.SendNewPasswordEmail(user, newPassword);
                return true;
            }
            catch (SecurityTokenException) {  throw; }
            catch (KeyNotFoundException) {  throw; }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task<bool> VerifyAccount(string token)
        {
            try
            {
                TokenVerify tokenVerify = await _tokenVerifyRepository.GetByToken(token, TokenType.EmailVerification);
                if (tokenVerify == null || tokenVerify.ExpirationDate < DateTime.UtcNow) throw new SecurityTokenException("Invalid token or expired");
                User user = await _userRepository.GetById(tokenVerify.UserId);
                if (user == null) throw new KeyNotFoundException($"User not found");
                user.IsActive = true;
                await _userRepository.Update(user);
                await _tokenVerifyRepository.Delete(tokenVerify);
                return true;
            }
            catch (SecurityTokenException)
            {
                throw;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred verifying the account: {ex.Message}", ex);
            }
        }

        private string GenerateVerificationToken()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData);
            }
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
