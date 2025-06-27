namespace G3NexusBackend.Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);
    Task<string> GetEmailTemplateAsync(string templateName);
}