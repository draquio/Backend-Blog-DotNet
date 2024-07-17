using BackendBlog.DTO.Image;

namespace BackendBlog.Service.Interface
{
    public interface IImageService
    {
        Task<List<ImageDetailDto>> GetPagedImages(int page, int pageSize);
        Task<ImageDetailDto> GetById(int id);
        Task<ImageDetailDto> Create(ImageUploadDto imageUploadDto);
        Task<bool> Update(ImageUpdateDto imageUpdateDto);
        Task<bool> Delete(int id);
    }
}
