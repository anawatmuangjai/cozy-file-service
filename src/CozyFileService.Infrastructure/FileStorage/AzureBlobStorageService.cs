using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CozyFileService.Application.Contracts.Infrastructure;

namespace CozyFileService.Infrastructure.FileStorage
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = blobContainerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, true);

            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string containerName, string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            var download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }

        public async Task<bool> DeleteFileAsync(string containerName, string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<bool> FileExistsAsync(string containerName, string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            return await blobClient.ExistsAsync();
        }

        public async Task<string> UpdateFileNameAsync(string containerName, string oldFileName, string newFileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var oldBlobClient = blobContainerClient.GetBlobClient(oldFileName);
            var newBlobClient = blobContainerClient.GetBlobClient(newFileName);

            // Check if the old blob exists
            if (!await oldBlobClient.ExistsAsync())
            {
                throw new ArgumentException($"Blob '{oldFileName}' does not exist in the container '{containerName}'.");
            }

            // Copy the contents of the old blob to the new blob
            await newBlobClient.StartCopyFromUriAsync(oldBlobClient.Uri);

            // Delete the old blob
            await oldBlobClient.DeleteIfExistsAsync();

            return newBlobClient.Uri.ToString();
        }
    }
}
