using CozyFileService.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CozyFileService.Identity
{
    public class CozyFileServiceIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public CozyFileServiceIdentityDbContext()
        {
        }

        public CozyFileServiceIdentityDbContext(DbContextOptions<CozyFileServiceIdentityDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging();
        }
    }
}
