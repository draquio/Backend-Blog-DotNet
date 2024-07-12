namespace BackendBlog.DTO.Response
{
    public class PagedResponse<T> : Response<T>
    {
        public int page { get; set; }
        public int pageSize { get; set; }
    }
}
