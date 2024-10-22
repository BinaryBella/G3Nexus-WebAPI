using System.Threading.Tasks;

namespace G3NexusBackend.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = false);
        Task<string> GetEmailTemplateAsync(string templateName);
    }
}