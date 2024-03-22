using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CozyFileService.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CozyFileService.Identity
{
    public static class IdentityServiceExtensions
    {
        public static void AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
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
