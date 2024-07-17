using BackendBlog.DTO.Category;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<CategoryListDto>>>> GetCategories(int page = 1, int pageSize = 10)
        {
            var rsp = new PagedResponse<List<CategoryListDto>>()
            {
                status = true,
                value = await _categoryService.GetPagedCategories(page, pageSize),
                page = page,
                pageSize = pageSize
            };
            return Ok(rsp);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response<CategoryReadDto>>> GetById(int id)
        {
            var rsp = new Response<CategoryReadDto>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _categoryService.GetById(id);
            return Ok(rsp);
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Response<CategoryReadDto>>> Create([FromBody] CategoryCreateDto category)
        {
            var rsp = new Response<CategoryReadDto>();
            if (category == null)
            {
                rsp.status = false;
                rsp.msg = "Category can't be null";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _categoryService.Create(category);
            return Ok(rsp);
        }
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Update([FromBody] CategoryUpdateDto category, int id)
        {
            var rsp = new Response<bool>();
            if(category == null || category.Id != id)
            {
                rsp.status = false;
                rsp.msg = "Category can't be null or ID mismatch";
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
            rsp.msg = "Category updated successfully";
            rsp.value = await _categoryService.Update(category);
            return Ok(rsp);
        }
        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response<bool>>> SoftDelete(int id)
        {
            var rsp = new Response<bool>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Category deleted successfully";
            rsp.value = await _categoryService.SoftDelete(id);
            return Ok(rsp);
        }

    }
}
