namespace BackendBlog.Model
{
    public partial class TokenHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string AccessToken {  get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public User User { get; set; }
    }
}
