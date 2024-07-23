using BackendBlog.DTO.Image;
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
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ImageDetailDto>> UploadImage([FromForm] ImageUploadDto imageUploadDto)
        {
            var rsp = new Response<ImageDetailDto>();
            if (imageUploadDto == null)
            {
                rsp.status = false;
                rsp.msg = "Image can't be null";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.msg = "Image uploaded successfully";
            rsp.value = await _imageService.Create(imageUploadDto);
            return Ok(rsp);
        }
        [HttpGet]
        public async Task<ActionResult<PagedResponse<List<ImageDetailDto>>>> GetImages(int page = 1, int pageSize = 20)
        {
            var rsp = new PagedResponse<List<ImageDetailDto>>()
            {
                status = true,
                value = await _imageService.GetPagedImages(page, pageSize),
                page = page,
                pageSize = pageSize
            };
            return Ok(rsp);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Response<ImageDetailDto>>> GetImage(int id)
        {
            var rsp = new Response<ImageDetailDto>();
            if (id <= 0)
            {
                rsp.status = false;
                rsp.msg = "Invalid ID";
                return BadRequest(rsp);
            }
            rsp.status = true;
            rsp.value = await _imageService.GetById(id);
            return Ok(rsp);
        }
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<bool>>> Update([FromBody] ImageUpdateDto imageUpdateDto, int id)
        {
            var rsp = new Response<bool>();
            if (imageUpdateDto == null || imageUpdateDto.Id != id)
            {
                rsp.status = false;
                rsp.msg = "Image can't be null or ID mismatch";
                return BadRequest(rsp);
            }

            rsp.status = true;
            rsp.msg = "Image updated successfully";
            rsp.value = await _imageService.Update(imageUpdateDto);
            return Ok(rsp);
        }
        [Authorize]
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
            rsp.msg = "Image deleted successfully";
            rsp.value = await _imageService.Delete(id);
            return Ok(rsp);
        }
    }
}
