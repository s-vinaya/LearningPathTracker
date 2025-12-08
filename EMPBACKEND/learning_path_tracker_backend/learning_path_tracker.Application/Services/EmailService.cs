using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using learning_path_tracker.Database;
using learning_path_tracker.Domain.Entities;

namespace learning_path_tracker.Application.Services;

public class EmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _appPassword;
    private readonly string _fromName;
    private readonly AppDbContext _context;

    public EmailService(IConfiguration configuration, AppDbContext context)
    {
        _smtpServer = configuration["Email:SmtpServer"]!;
        _smtpPort = int.Parse(configuration["Email:SmtpPort"]!);
        _fromEmail = configuration["Email:FromEmail"]!;
        _appPassword = configuration["Email:AppPassword"]!;
        _fromName = configuration["Email:FromName"]!;
        _context = context;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailLog = new EmailLog
        {
            ToEmail = toEmail,
            Subject = subject,
            SentAt = DateTime.UtcNow
        };

        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_fromName, _fromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_fromEmail, _appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            emailLog.IsSuccess = true;
        }
        catch (Exception ex)
        {
            emailLog.IsSuccess = false;
            emailLog.ErrorMessage = ex.Message;
        }
        finally
        {
            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();
        }
    }


}
