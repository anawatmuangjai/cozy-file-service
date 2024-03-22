using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Models.Mail;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace CozyFileService.Infrastructure.Mail
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailSettings> _logger { get;  }

        public EmailService(IOptions<EmailSettings> mailSettings, ILogger<EmailSettings> logger)
        {
            _emailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(Email email)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);

            var subject = email.Subject;
            var to = new EmailAddress(email.To);
            var emailBody = email.Body;

            var from = new EmailAddress
            {
                Email = _emailSettings.FromAddress,
                Name = _emailSettings.FromName,
            };

            var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);

            var response = await client.SendEmailAsync(sendGridMessage);

            _logger.LogInformation("Email sent");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            _logger.LogError("Email sending failed");

            return false;
        }
    }
}
