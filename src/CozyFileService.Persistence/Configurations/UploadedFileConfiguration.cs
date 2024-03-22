using CozyFileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CozyFileService.Persistence.Configurations
{
    public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
    {
        public void Configure(EntityTypeBuilder<UploadedFile> builder)
        {
            builder.ToTable("UploadedFiles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.FileType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FilePath).IsRequired().HasMaxLength(200);
            builder.Property(x => x.FileSize).IsRequired();
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
            builder.Property(x => x.LastModifiedDate);
        }
    }
}
