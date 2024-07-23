using BackendBlog.DTO.Post;
using BackendBlog.DTO.Response;
using BackendBlog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<PostListDto>>>> GetPosts(int page = 1, int pageSize = 10, bool? IsPublished = null)
        {
            var rsp = new PagedResponse<List<PostListDto>>()
            {
                status = true,
                value = await _postService.GetPagedPosts(page, pageSize, IsPublished),
                page = page,
                pageSize = pageSize
            };
            return Ok(rsp);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response<PostReadDto>>> GetPost(int id)
        {
            var rsp = new Response<PostReadDto>();
            rsp.status = true;
            rsp.value = await _postService.GetPostWithData(id);
            return Ok(rsp);
        }
        [Authorize(Roles = "Administrador,Editor")]
        [HttpPost]
        public async Task<ActionResult<Response<PostReadDto>>> Create([FromBody] PostCreateDto post)
        {
            var rsp = new Response<PostReadDto>();
            if (post == null)
            {
                rsp.status = false;
                rsp.msg = "Post can't be null";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Post created successfully";
            rsp.value = await _postService.Create(post);
            return Ok(rsp);
        }
        [Authorize(Roles = "Administrador,Editor")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Update([FromBody] PostUpdateDto post, int id)
        {
            var rsp = new Response<bool>();
            if (post == null || post.Id != id)
            {
                rsp.status = false;
                rsp.msg = "Post can't be null or ID mismatch";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Post updated successfully";
            rsp.value = await _postService.Update(post);
            return Ok(rsp);
        }
        [Authorize(Roles = "Administrador,Editor")]
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
            rsp.msg = "Post deleted successfully";
            rsp.value = await _postService.Delete(id);
            return Ok(rsp);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<Response<List<PostListDto>>>> GetFilteredPosts([FromQuery] int? categoryId, [FromQuery] int? userId, 
            [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? tag)
        {
            var rsp = new Response<List<PostListDto>>();
            rsp.status = true;
            rsp.value = await _postService.GetFilteredPosts(categoryId, userId, startDate, endDate, tag);
            return Ok(rsp);
        }

        [HttpGet("search")]
        public async Task<ActionResult<Response<List<PostListDto>>>> SearchPosts([FromQuery] string term)
        {
            var rsp = new Response<List<PostListDto>>();
            if (string.IsNullOrEmpty(term))
            {
                rsp.status = false;
                rsp.msg = "Invalid term";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _postService.SearchPosts(term);
            return Ok(rsp);
        }
    }
}
