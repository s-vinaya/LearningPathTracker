namespace EMPBACKEND.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendOtpEmailAsync(string toEmail, string otp);
        Task SendPasswordResetConfirmationAsync(string toEmail);
        Task SendWelcomeEmailAsync(string toEmail, string firstName, string lastName);
        Task SendApprovalEmailAsync(string toEmail, string firstName, string lastName);
        Task SendRejectionEmailAsync(string toEmail, string firstName, string lastName);
    }
}