using G3NexusBackend.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace G3NexusBackend.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("vmsfleetpulse@gmail.com", "oxhmpkvadaabxrqn"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your-email@example.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,  // Enable HTML if needed, otherwise set to false
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
