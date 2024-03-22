using CozyFileService.Domain.Entities;

namespace CozyFileService.Application.Contracts.Persistance
{
    public interface IUploadedFileRespository : IAsyncRepository<UploadedFile>
    {
    }
}
