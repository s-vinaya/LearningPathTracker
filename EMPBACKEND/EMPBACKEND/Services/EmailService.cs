using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using EMPBACKEND.Utilities;

namespace EMPBACKEND.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string email, string otpCode);
        Task SendPasswordResetConfirmationAsync(string email);
        Task SendCourseAssignmentEmailAsync(string email, string courseName);
        Task SendWelcomeEmailAsync(string email, string firstName, string lastName);
        Task SendApprovalEmailAsync(string email, string firstName, string lastName);
        Task SendRejectionEmailAsync(string email, string firstName, string lastName);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string email, string otpCode)
        {
            var body = string.Format(EmailConstants.Bodies.OTP, otpCode);
            await SendEmailAsync(email, EmailConstants.Subjects.OTP, body);
        }

        public async Task SendPasswordResetConfirmationAsync(string email)
        {
            await SendEmailAsync(email, EmailConstants.Subjects.PasswordResetConfirmation, EmailConstants.Bodies.PasswordResetConfirmation);
        }

        public async Task SendCourseAssignmentEmailAsync(string email, string courseName)
        {
            var body = string.Format(EmailConstants.Bodies.CourseAssignment, courseName);
            await SendEmailAsync(email, EmailConstants.Subjects.CourseAssignment, body);
        }

        public async Task SendWelcomeEmailAsync(string email, string firstName, string lastName)
        {
            var body = string.Format(EmailConstants.Bodies.Welcome, firstName, lastName);
            await SendEmailAsync(email, EmailConstants.Subjects.Welcome, body);
        }

        public async Task SendApprovalEmailAsync(string email, string firstName, string lastName)
        {
            var body = string.Format(EmailConstants.Bodies.Approval, firstName, lastName);
            await SendEmailAsync(email, EmailConstants.Subjects.Approval, body);
        }

        public async Task SendRejectionEmailAsync(string email, string firstName, string lastName)
        {
            var body = string.Format(EmailConstants.Bodies.Rejection, firstName, lastName);
            await SendEmailAsync(email, EmailConstants.Subjects.Rejection, body);
        }

        private async Task SendEmailAsync(string email, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Employee Learning Portal", _configuration["Email:FromEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["Email:SmtpServer"], int.Parse(_configuration["Email:SmtpPort"]!), SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration["Email:FromEmail"], _configuration["Email:AppPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}