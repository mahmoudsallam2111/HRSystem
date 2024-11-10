using HRSystem.Application.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HRSystem.Infrastructure.Persistence.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse(_emailSettings.FromEmail));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient ())
            {
                client.Connect(_emailSettings.MailServer, _emailSettings.MailPort, SecureSocketOptions.StartTls);
                client.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
                client.Send(emailToSend);
                client.Disconnect(true);
            }
            return Task.CompletedTask;
        }
    }
}
