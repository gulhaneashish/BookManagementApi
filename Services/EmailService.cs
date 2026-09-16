using BookApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace BookApi.Services
{
    public class EmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(
            IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body,
            byte[]? attachment = null,
            string? attachmentName = null)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "BookStore API",
                    _settings.Username
                )
            );

            message.To.Add(
                MailboxAddress.Parse(to)
            );

            message.Subject = subject;

            var builder = new BodyBuilder
            {
                TextBody = body
            };

            if (attachment != null && attachmentName != null)
            {
                builder.Attachments.Add(
                    attachmentName,
                    attachment
                );
            }

            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            await smtp.ConnectAsync(
                _settings.Host,
                _settings.Port,
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _settings.Username,
                _settings.Password
            );

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}