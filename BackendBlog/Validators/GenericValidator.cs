using System.Text.RegularExpressions;

namespace BackendBlog.Validators
{
    public static class GenericValidator
    {
        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or whitespace.");
            }

            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("The provided email is not valid.");
            }
        }
    }
}
