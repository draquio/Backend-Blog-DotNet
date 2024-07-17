using BackendBlog.Model;

namespace BackendBlog.Repository.Interface
{
    public interface IImageRepository : IGenericRepository<Image>
    {
        Task<List<Image>> GetPagedImages(int page, int pageSize);
    }
}
