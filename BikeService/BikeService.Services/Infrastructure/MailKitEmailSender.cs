using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Text;
using System.Threading.Tasks;

namespace BikeService.Services.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailKitEmailSender> _logger;

        public MailKitEmailSender(IConfiguration configuration, ILogger<MailKitEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["MailKit:SenderName"],
                _configuration["MailKit:SenderEmail"]
            ));

            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _configuration["MailKit:Server"],
                int.Parse(_configuration["MailKit:Port"]),
                MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(
                _configuration["MailKit:Account"],
                _configuration["MailKit:Password"]);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation($"Email sent to {email} with subject {subject}");
        }
    }
}
