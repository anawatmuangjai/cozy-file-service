namespace CozyFileService.Application.Contracts.Infrastructure
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream);
        Task<Stream> DownloadFileAsync(string containerName, string fileName);
        Task<bool> DeleteFileAsync(string containerName, string fileName);
        Task<bool> FileExistsAsync(string containerName, string fileName);
        Task<string> UpdateFileNameAsync(string containerName, string oldFileName, string newFileName);
    }
}
