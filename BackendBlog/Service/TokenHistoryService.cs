using BackendBlog.DTO.Token;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace BackendBlog.Service
{
    public class TokenHistoryService : ITokenHistoryService
    {
        private readonly ITokenHistoryRepository _tokenRepository;
        private readonly IConfiguration _configuration;

        public TokenHistoryService(ITokenHistoryRepository tokenRepository, IConfiguration configuration)
        {
            _tokenRepository = tokenRepository;
            _configuration = configuration;
        }

        public async Task<TokenReadDto> GenerateToken(User user)
        {
            try
            {
                var accessToken = GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();
                var tokenHistory = new TokenHistory
                {
                    UserId = user.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpirationDate = DateTime.UtcNow.AddDays(1)
                };
                await _tokenRepository.Create(tokenHistory);
                return new TokenReadDto { AccessToken = accessToken, RefreshToken = refreshToken };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task<TokenReadDto> RefreshToken(string refreshToken)
        {
            try
            {
                var tokenHistory = await _tokenRepository.GetByRefreshToken(refreshToken);
                if(tokenHistory == null || tokenHistory.ExpirationDate <= DateTime.UtcNow)
                {
                    throw new SecurityTokenException("Invalid refresh token or expired");
                }
                User user = tokenHistory.User;
                var newAccessToken = GenerateAccessToken(user);
                var newRefreshToken = GenerateRefreshToken();
                tokenHistory.AccessToken = newAccessToken;
                tokenHistory.RefreshToken = newRefreshToken;
                tokenHistory.CreatedAt = DateTime.UtcNow;
                tokenHistory.ExpirationDate = DateTime.UtcNow.AddDays(1);
                await _tokenRepository.Update(tokenHistory);
                return new TokenReadDto { AccessToken = newAccessToken, RefreshToken = refreshToken };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                 new Claim(ClaimTypes.Name, user.Username),
                 new Claim(ClaimTypes.Email, user.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims, "Custom"),
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = creds,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
