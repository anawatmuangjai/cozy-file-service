using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CozyFileService.Persistence
{
    public static class PersistanceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:Database"];
            services.AddDbContext<CozyFileServiceDbContext>(options =>
                           options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IUploadedFileRepository, UploadedFileRepository>();

            return services;
        }
    }
}
