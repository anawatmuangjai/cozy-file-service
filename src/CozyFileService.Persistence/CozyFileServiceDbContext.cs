﻿using CozyFileService.Application.Contracts;
using CozyFileService.Domain.Common;
using CozyFileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CozyFileService.Persistence
{
    public class CozyFileServiceDbContext : DbContext
    {
        private readonly ILoggedInUserService _loggedInUserService;

        public CozyFileServiceDbContext(DbContextOptions<CozyFileServiceDbContext> options) : base(options)
        {
        }

        public CozyFileServiceDbContext(DbContextOptions<CozyFileServiceDbContext> options, ILoggedInUserService loggedInUserService) : base(options)
        {
            _loggedInUserService = loggedInUserService;
        }

        public DbSet<UploadedFile> UploadedFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CozyFileServiceDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = _loggedInUserService.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = _loggedInUserService.UserId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
