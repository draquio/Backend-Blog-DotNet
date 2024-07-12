using BackendBlog.DTO.Response;
using BackendBlog.DTO.User;
using BackendBlog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBlog.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<UserListDto>>>> GetUsers(int page = 1, int pageSize = 10)
        {
            var rsp = new PagedResponse<List<UserListDto>>()
            {
                status = true,
                value = await _userService.GetPagedUsers(page, pageSize),
                page = page,
                pageSize = pageSize
            };
            return Ok(rsp);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response<UserReadDto>>> GetUser(int id)
        {
            var rsp = new Response<UserReadDto>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _userService.GetById(id);
            return Ok(rsp);
        }
        [HttpPost]
        public async Task<ActionResult<Response<UserReadDto>>> Create([FromBody] UserCreateDto user)
        {
            var rsp = new Response<UserReadDto>();
            if (user == null)
            {
                rsp.status = false;
                rsp.msg = "User can't be null";
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
            rsp.value = await _userService.Create(user);
            return Ok(rsp);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Update([FromBody] UserUpdateDto user, int id)
        {
            var rsp = new Response<bool>();
            if (user == null || user.Id != id)
            {
                rsp.status = false;
                rsp.msg = "User can't be null or ID mismatch";
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
            rsp.msg = "User updated successfully";
            rsp.value = await _userService.Update(user);
            return Ok(rsp);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Delete(int id)
        {
            var rsp = new Response<bool>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "User deleted successfully";
            rsp.value = await _userService.Delete(id);
            return Ok(rsp);
        }
        [HttpPatch("update-password")]
        public async Task<ActionResult<Response<bool>>> ChangePassword([FromBody] UserChangePasswordDto changePassword)
        {
            var rsp = new Response<bool>();
            if (changePassword.Id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
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
            rsp.msg = "Password updated successfully";
            rsp.value = await _userService.ChangePassword(changePassword);
            return Ok(rsp);
        }
    }
}
