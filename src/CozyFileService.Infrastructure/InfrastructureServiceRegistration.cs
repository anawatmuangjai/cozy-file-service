using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Models.Mail;
using CozyFileService.Infrastructure.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CozyFileService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettings);

            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
