using BackendBlog.Model;
using BackendBlog.Service.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BackendBlog.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendNewPasswordEmail(User user, string newPassword)
        {
            try
            {
                var emailBody = $@"
                    <html>
                    <body>
                        <p>Dear {user.Username}, your password has been reset.</p>
                        <p>New Password: {newPassword}</p>
                        <p>Login into the web and change this password for security.</p>
                    </body>
                    </html>";


                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Blog", _configuration["Smtp:FromEmail"]));
                message.To.Add(new MailboxAddress(user.Username, user.Email));
                message.Subject = "New password generated";
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                await SendEmail(message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task SendPasswordResetEmail(User user, string token)
        {
            try
            {
                var encodedToken = WebUtility.UrlEncode(token);
                string resetLink = $"https://localhost:7071/api/Auth/reset-password?token={encodedToken}";
                string clickableLink = $"<a href=\"{resetLink}\">Click here to reset the your account password.</a>";
                var emailBody = $@"
                    <html>
                    <body>
                        <p>Reseting Password. Please click the link below to reset the passward account:</p>
                        <p>{clickableLink}</p>
                        <p>After click on the link you will receive your new password on your email</p>
                    </body>
                    </html>";


                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Blog", _configuration["Smtp:FromEmail"]));
                message.To.Add(new MailboxAddress(user.Username, user.Email));
                message.Subject = "Password Reset";
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                await SendEmail(message);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        public async Task SendVerificationEmail(User user, string token)
        {
			try
			{
                var encodedToken = WebUtility.UrlEncode(token);
                string verificationLink = $"https://localhost:7071/api/Auth/verify?token={encodedToken}";
                string clickableLink = $"<a href=\"{verificationLink}\">Click here to verify your account</a>";
                var emailBody = $@"
                    <html>
                    <body>
                        <p>Thank you for registering. Please click the link below to verify your account:</p>
                        <p>{clickableLink}</p>
                    </body>
                    </html>";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Blog", _configuration["Smtp:FromEmail"]));
                message.To.Add(new MailboxAddress(user.Username, user.Email));
                message.Subject = "Email Verification";
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                await SendEmail(message);
            }
			catch (Exception ex)
			{
                throw new ApplicationException($"An error occurred: {ex.Message}", ex);
            }
        }

        private async Task SendEmail(MimeMessage message)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    var host = _configuration["Smtp:Host"];
                    var port = int.Parse(_configuration["Smtp:Port"]);
                    var username = _configuration["Smtp:FromEmail"];
                    var password = _configuration["Smtp:Password"];

                    await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(username, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
