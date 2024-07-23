using BackendBlog.DTO.Comment;
using BackendBlog.DTO.Response;
using BackendBlog.Model;
using BackendBlog.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackendBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<CommentDetailDto>>>> GetComments(int page = 1, int pageSize = 10)
        {
            var rsp = new PagedResponse<List<CommentDetailDto>>()
            {
                status = true,
                value = await _commentService.GetPagedComments(page, pageSize),
                page = page,
                pageSize = pageSize
            };
            return Ok(rsp);
        }

        [HttpGet("public-comments/{postId:int}")]
        public async Task<ActionResult<PagedResponse<List<CommentDetailDto>>>> GetCommentsByPostId(int postId, int page = 1, int pageSize = 10)
        {
            var rsp = new PagedResponse<List<CommentDetailDto>>();
            if (postId <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            bool isApproved = true;
            rsp.status = true;
            rsp.value = await _commentService.GetPagedCommentsByPostId(postId, page, pageSize, isApproved);
            rsp.page = page;
            rsp.pageSize = pageSize;
            return Ok(rsp);
        }

        [Authorize]
        [HttpGet("{postId:int}")]
        public async Task<ActionResult<PagedResponse<List<CommentDetailDto>>>> GetCommentsByPostId(int postId, int page = 1, int pageSize = 10, bool? isApproved = null)
        {
            var rsp = new PagedResponse<List<CommentDetailDto>>();
            if (postId <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _commentService.GetPagedCommentsByPostId(postId, page, pageSize, isApproved);
            rsp.page = page;
            rsp.pageSize = pageSize;
            return Ok(rsp);
        }

        [HttpPost]
        public async Task<ActionResult<Response<CommentDetailDto>>> Create([FromBody] CommentCreateDto commentCreateDto)
        {
            var rsp = new Response<CommentDetailDto>();
            rsp.status = true;
            rsp.msg = "Comment created successfully";
            rsp.value = await _commentService.Create(commentCreateDto);
            return Ok(rsp);
        }
        [Authorize(Roles = "Administrador,Editor")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Update([FromBody] CommentUpdateDto commentUpdateDto, int id)
        {
            var rsp = new Response<bool>();
            if (commentUpdateDto == null || commentUpdateDto.Id != id)
            {
                rsp.status = false;
                rsp.msg = "Category can't be null or ID mismatch";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Comment updated successfully";
            rsp.value = await _commentService.Update(commentUpdateDto);
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
            rsp.msg = "Comment deleted successfully";
            rsp.value = await _commentService.Delete(id);
            return Ok(rsp);
        }
        [Authorize(Roles = "Administrador,Editor")]
        [HttpPatch("approve/{id:int}")]
        public async Task<ActionResult<Response<bool>>> ApproveComment(int id)
        {
            var rsp = new Response<bool>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Comment approved successfully";
            rsp.value = await _commentService.ApproveComment(id);
            return Ok(rsp);
        }
    }
}
