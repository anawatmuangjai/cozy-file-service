﻿using AutoMapper;
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

            var containerName = _loggedInUserService.UserId;
            var isFileExists = await _fileStorageService.FileExistsAsync(containerName, fileToDelete.FileName);
            if (!isFileExists)
            {
                _logger.LogInformation($"Not found file name {fileToDelete.FileName} in blob storage.");
                throw new NotFoundException(nameof(UploadedFile), fileToDelete.FileName);
            }

            var isDeleted = await _fileStorageService.DeleteFileAsync(containerName, fileToDelete.FileName);
            if (isDeleted)
            {
                _logger.LogInformation($"Deleted file {fileToDelete.FileName} from blob storage successfully.");

                await _uploadedFileRepository.DeleteAsync(fileToDelete);
                _logger.LogInformation($"Deleted file name {fileToDelete.FileName} from database successfully.");
            }
        }
    }
}
