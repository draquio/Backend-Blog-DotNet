using AutoMapper;
using BackendBlog.DTO.Category;
using BackendBlog.DTO.Image;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;
using BackendBlog.Validators.Pagination;

namespace BackendBlog.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webHostEnvironment;

        public ImageService(IImageRepository imageRepository, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<ImageDetailDto>> GetPagedImages(int page, int pageSize)
        {
            try
            {
                PaginationValidator.ValidatePage(page);
                PaginationValidator.ValidatePageSize(pageSize);
                List<Image> images = await _imageRepository.GetPagedImages(page, pageSize);
                if (images == null) return new List<ImageDetailDto>();
                List<ImageDetailDto> imageDetailDto = _mapper.Map<List<ImageDetailDto>>(images);
                return imageDetailDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Images: {ex.Message}", ex);
            }
        }
        public async Task<ImageDetailDto> GetById(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                Image image = await _imageRepository.GetById(id);
                if(image == null) throw new KeyNotFoundException($"Image with ID {id} not found");
                ImageDetailDto imageDetailDto = _mapper.Map<ImageDetailDto>(image);
                return imageDetailDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the image: {ex.Message}", ex);
            }
        }
        public async Task<ImageDetailDto> Create(ImageUploadDto imageUploadDto)
        {
            try
            {
                string url = await SaveImage(imageUploadDto.File);
                Image image = new Image()
                {
                    Url = url,
                    AltText = imageUploadDto.File.FileName,
                    CreatedAt = DateTime.Now,
                };
                await _imageRepository.Create(image);
                ImageDetailDto imageDetailDto = _mapper.Map<ImageDetailDto>(image);
                return imageDetailDto;

            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while saving the image: {ex.Message}", ex);
            }
        }
        public async Task<bool> Update(ImageUpdateDto imageUpdateDto)
        {
            try
            {
                Image image = await _imageRepository.GetById(imageUpdateDto.Id);
                if(image == null) throw new KeyNotFoundException($"Image with ID {imageUpdateDto.Id} not found");
                image.AltText = imageUpdateDto.AltText;
                bool response = await _imageRepository.Update(image);
                if (!response) throw new InvalidOperationException("Image couldn't be updated");
                return response;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while updating image: {ex.Message}", ex);
            }
        }
        public async Task<bool> Delete(int id)
        {
            try
            {
                IdValidator.ValidateId(id);
                Image image = await _imageRepository.GetById(id);
                if (image == null) throw new KeyNotFoundException($"Image with ID {id} not found");
                DeleteImageFile(image.Url);
                bool response = await _imageRepository.Delete(image);
                if (!response) throw new InvalidOperationException("Image couldn't be updated");
                return response;

            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting image: {ex.Message}", ex);
            }
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            var uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            var uniqueFileName = Guid.NewGuid().ToString() +  "_" + file.FileName;
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/Images/{uniqueFileName}";
        }
        private void DeleteImageFile(string url)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, url.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
