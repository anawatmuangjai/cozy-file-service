using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Domain.Entities;

namespace CozyFileService.Persistence.Repositories
{
    public class UploadedFileRepository : BaseRepository<UploadedFile>, IUploadedFileRepository
    {
        public UploadedFileRepository(CozyFileServiceDbContext dbContext) : base(dbContext)
        {
        }
    }
}
