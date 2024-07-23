using BackendBlog.DTO.Response;
using BackendBlog.DTO.Auth;
using BackendBlog.DTO.Token;
using BackendBlog.Model;
using Microsoft.AspNetCore.Mvc;
using BackendBlog.Service.Interface;
using BackendBlog.DTO.User;

namespace BackendBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
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

            rsp.status = true;
            User user = await _authService.Login(login);
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

            rsp.status = true;
            rsp.value = await _authService.Register(register);
            rsp.msg = "Registered Successfully";
            return Ok(rsp);
        }

        [HttpGet("verify")]
        public async Task<ActionResult<Response<bool>>> VerifyAccount([FromQuery] string token)
        {
            var rsp = new Response<bool>();
            //string decodedToken = WebUtility.UrlDecode(token);
            rsp.status = true;
            rsp.msg = "Account verified successfully";
            rsp.value = await _authService.VerifyAccount(token);
            return Ok(rsp);
        }

        [HttpPost("request-password-reset")]
        public async Task<ActionResult<Response<bool>>> RequestPasswordReset([FromBody] ResetPasswordRequestDto requestReset)
        {
            var rsp = new Response<bool>();
            rsp.status = true;
            rsp.msg = "Email to reset password was sent";
            rsp.value = await _authService.RequestPasswordReset(requestReset);
            return Ok(rsp);
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<Response<bool>>> ResetPassword([FromQuery] string token)
        {
            var rsp = new Response<bool>();
            // Si el token entra por la url como debe ser en POST, no hace falta hacer decode del Token
            // string decodedToken = WebUtility.UrlDecode(token);
            rsp.status = true;
            rsp.msg = "Password reset successfully";
            rsp.value = await _authService.ResetPassword(token);
            return Ok(rsp);
        }

    }
}
