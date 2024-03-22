using AutoMapper;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Exceptions;
using CozyFileService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteFileCommandHandler> _logger;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public DeleteFileCommandHandler(IMapper mapper,
            ILogger<DeleteFileCommandHandler> logger,
            IUploadedFileRepository uploadedFileRepository,
            IFileStorageService fileStorageService,
            ILoggedInUserService loggedInUserService)
        {
            _mapper = mapper;
            _logger = logger;
            _uploadedFileRepository = uploadedFileRepository;
            _fileStorageService = fileStorageService;
            _loggedInUserService = loggedInUserService;
        }

        public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var fileToDelete = await _uploadedFileRepository.GetByIdAsync(request.Id);

            if (fileToDelete == null)
            {
                throw new NotFoundException(nameof(UploadedFile), request.Id);
            }

            var isFileExist = await _fileStorageService.FileExistsAsync(_loggedInUserService.UserName, fileToDelete.FilePath);

            if (isFileExist)
            {
                var isDeleted = await _fileStorageService.DeleteFileAsync(_loggedInUserService.UserName, fileToDelete.FilePath);

                if (isDeleted)
                {
                    _logger.LogInformation($"Deleted file {fileToDelete.FileName} from blob storage successfully.");

                    await _uploadedFileRepository.DeleteAsync(fileToDelete);
                }
            }
        }
    }
}
