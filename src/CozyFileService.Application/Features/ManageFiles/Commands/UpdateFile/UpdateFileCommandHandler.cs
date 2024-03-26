using AutoMapper;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Exceptions;
using CozyFileService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile
{
    public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateFileCommandHandler> _logger;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public UpdateFileCommandHandler(IMapper mapper, 
            ILogger<UpdateFileCommandHandler> logger, 
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

        public async Task Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var fileToUpdate = await _uploadedFileRepository.GetByIdAsync(request.Id);

            if (fileToUpdate == null)
            {
                throw new NotFoundException(nameof(UploadedFile), request.Id);
            }

            var validator = new UpdateFileCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.Errors.Count > 0)
            {
                throw new ValidationException(validationResult);
            }

            var oldFilename = fileToUpdate.FileName;
            var newFileName = request.FileName;
            var containerName = _loggedInUserService.UserId;

            var isFileExists = await _fileStorageService.FileExistsAsync(containerName, oldFilename);

            if (!isFileExists)
            {
                _logger.LogInformation($"Not found file name {oldFilename} in blob storage.");
                throw new NotFoundException(nameof(UploadedFile), oldFilename);
            }

            var newFilePath = await _fileStorageService.UpdateFileNameAsync(containerName, oldFilename, newFileName);

            _logger.LogInformation($"Update file name {oldFilename} to {newFileName} in blob storage successfully.");

            fileToUpdate.FilePath = newFilePath;

            _mapper.Map(request, fileToUpdate, typeof(UpdateFileCommand), typeof(UploadedFile));

            await _uploadedFileRepository.UpdateAsync(fileToUpdate);
        }
    }
}
