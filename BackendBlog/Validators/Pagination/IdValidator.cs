namespace BackendBlog.Validators.Pagination
{
    public static class IdValidator
    {
        public static void ValidateId(int id)
        {
            if (id < 1) throw new ArgumentException("Id must be greater than or equal to 1.");
        }
    }
}
