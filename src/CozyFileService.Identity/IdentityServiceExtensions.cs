using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CozyFileService.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CozyFileService.Identity
{
    public static class IdentityServiceExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();

            services.AddAuthorizationBuilder();

            var connectionString = configuration["ConnectionStrings:CozyFileServiceIdentity"];
            services.AddDbContext<CozyFileServiceIdentityDbContext>(options =>
                           options.UseSqlServer(connectionString));

            services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<CozyFileServiceIdentityDbContext>()
                .AddApiEndpoints();
        }
    }
}
