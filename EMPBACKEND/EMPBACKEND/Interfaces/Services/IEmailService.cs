namespace EMPBACKEND.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendOtpAsync(string toEmail, string otp);
        Task SendPasswordResetAsync(string toEmail, string resetLink);
        Task SendCourseAssignmentAsync(string toEmail, string courseName);
        Task SendApprovalEmailAsync(string toEmail, string firstName, string lastName);
        Task SendRejectionEmailAsync(string toEmail, string firstName, string lastName);
    }
}