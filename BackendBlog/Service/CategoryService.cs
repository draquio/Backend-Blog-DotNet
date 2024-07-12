using AutoMapper;
using BackendBlog.DTO.Category;
using BackendBlog.DTO.User;
using BackendBlog.Model;
using BackendBlog.Repository.Interface;
using BackendBlog.Service.Interface;

namespace BackendBlog.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryListDto>> GetPagedCategories(int page, int pageSize)
        {
            try
            {
                List<Category> categories = await _categoryRepository.GetPagedCategories(page, pageSize);
                if (categories == null) return new List<CategoryListDto>();
                List<CategoryListDto> categoryListDto = _mapper.Map<List<CategoryListDto>>(categories);
                return categoryListDto;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while retrieving the list of Categories: {ex.Message}", ex);
            }
        }
        public async Task<CategoryReadDto> GetById(int id)
        {
            try
            {
                Category category = await _categoryRepository.GetById(id);
                if(category == null) throw new KeyNotFoundException($"Category with ID {id} not found");
                CategoryReadDto categoryReadDto = _mapper.Map<CategoryReadDto>(category);
                return categoryReadDto;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {

                throw new ApplicationException($"An error occurred while retrieving the category: {ex.Message}", ex);
            }
        }
        public async Task<CategoryReadDto> Create(CategoryCreateDto categoryCreateDto)
        {
            try
            {
                Category category = _mapper.Map<Category>(categoryCreateDto);
                Category categoryCreated = await _categoryRepository.Create(category);
                if (categoryCreated == null || categoryCreated.Id == 0) throw new InvalidOperationException("Category couldn't be created");
                CategoryReadDto categoryReadDto = _mapper.Map<CategoryReadDto>(categoryCreated);
                return categoryReadDto;
            }
            catch
            {

                throw;
            }
        }
        public async Task<bool> Update(CategoryUpdateDto categoryUpdateDto)
        {
            try
            {
                Category category = await _categoryRepository.GetById(categoryUpdateDto.Id);
                if(category == null) throw new KeyNotFoundException($"Category with ID {categoryUpdateDto.Id} not found");
                _mapper.Map(categoryUpdateDto, category);
                bool response = await _categoryRepository.Update(category);
                if(!response) throw new InvalidOperationException("Category couldn't be updated");
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
                throw new ApplicationException($"An error occurred while updating category: {ex.Message}", ex);
            }
        }
        public async Task<bool> SoftDelete(int id)
        {
            try
            {
                Category category = await _categoryRepository.GetById(id);
                if(category == null) throw new KeyNotFoundException($"Category with ID {id} not found");
                category.IsDelete = true;
                bool response = await _categoryRepository.Update(category);
                if(!response) throw new InvalidOperationException("Category couldn't be deleted");
                return response;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while deleting category: {ex.Message}", ex);
            }
        }

    }
}
