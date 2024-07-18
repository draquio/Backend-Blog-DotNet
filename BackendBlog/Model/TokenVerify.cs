namespace BackendBlog.Model
{
    public class TokenVerify
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; }
        public TokenType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public User User { get; set; }
    }

    public enum TokenType
    {
        EmailVerification,
        PasswordReset
    }
}
