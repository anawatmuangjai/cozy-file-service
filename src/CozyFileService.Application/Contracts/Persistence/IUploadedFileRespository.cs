using CozyFileService.Domain.Entities;

namespace CozyFileService.Application.Contracts.Persistence
{
    public interface IUploadedFileRepository : IAsyncRepository<UploadedFile>
    {
    }
}
