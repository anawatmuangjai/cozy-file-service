using CozyFileService.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
