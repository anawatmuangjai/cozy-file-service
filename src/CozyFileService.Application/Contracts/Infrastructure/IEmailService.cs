using CozyFileService.Application.Models.Mail;

namespace CozyFileService.Application.Contracts.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Email email);
    }
}
