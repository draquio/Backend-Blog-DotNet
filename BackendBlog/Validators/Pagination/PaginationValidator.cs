namespace BackendBlog.Validators.Pagination
{
    public static class PaginationValidator
    {
        public static void ValidatePage(int page)
        {
            if(page < 1) throw new ArgumentException("Page must be greater than or equal to 1.");
        }
        public static void ValidatePageSize(int pageSize)
        {
            int maxPageSize = 20;
            if (pageSize < 1) throw new ArgumentException("Page size must be greater than or equal to 1.");
            if(pageSize > maxPageSize) throw new ArgumentException($"Page size must be less than or equal to {maxPageSize}.");
        }
    }
}
