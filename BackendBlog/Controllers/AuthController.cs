using BackendBlog.DTO.Response;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Token;
using BackendBlog.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.DTO.User;

namespace BackendBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //private readonly IConfiguration _config;
        private readonly ITokenHistoryService _tokenService;
        private readonly IAuthService _authService;

        public AuthController(ITokenHistoryService tokenService, IAuthService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response<string>>> Login([FromBody] LoginDto login)
        {
            var rsp = new Response<TokenReadDto>();
            if (login == null)
            {
                rsp.status = false;
                rsp.msg = "Login data can't be null";
                return BadRequest(rsp);
            }
            if (!ModelState.IsValid)
            {
                rsp.status = false;
                rsp.msg = "Invalid data";
                rsp.errors = ModelState.Values
                    .SelectMany(err => err.Errors)
                    .Select(err => err.ErrorMessage)
                    .ToList();
                return BadRequest(rsp);
            }

            rsp.status = true;
            User user = await _authService.Login(login);
            //User user = new User() { Username = "draquio", Email = "draquio@gmail.com", Password = "123", Id = 1 };
            rsp.value = await _tokenService.GenerateToken(user);
            return Ok(rsp);
        }

        [HttpPost("refreshtoken")]
        public async Task<ActionResult<Response<TokenReadDto>>> RefreshToken([FromBody] TokenRefreshDto refreshToken)
        {
            var rsp = new Response<TokenReadDto>();
            rsp.status = true;
            rsp.value = await _tokenService.RefreshToken(refreshToken.RefreshToken);
            return Ok(rsp);
        }
        [HttpPost("register")]
        public async Task<ActionResult<Response<UserListDto>>> Register([FromBody] RegisterDto register)
        {
            var rsp = new Response<UserListDto>();
            if (register == null)
            {
                rsp.status = false;
                rsp.msg = "Register data can't be null";
                return BadRequest(rsp);
            }
            if (!ModelState.IsValid)
            {
                rsp.status = false;
                rsp.msg = "Invalid data";
                rsp.errors = ModelState.Values
                    .SelectMany(err => err.Errors)
                    .Select(err => err.ErrorMessage)
                    .ToList();
                return BadRequest(rsp);
            }

            rsp.status = true;
            rsp.value = await _authService.Register(register);
            rsp.msg = "Registered Successfully";
            return Ok(rsp);
        }

        [Authorize]
        [HttpGet("prueba")]
        public async Task<IActionResult> GetList()
        {
            List<string> lista = new List<string>()
            {
                "valor1", "valor2", "valor3",
            };
            return Ok(lista);
        }

    }
}
