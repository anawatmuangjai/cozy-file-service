using AutoMapper;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Features.ManageFiles.Commands.CreateFile;
using MediatR;
using Microsoft.Extensions.Logging;
using CozyFileService.Application.Exceptions;

namespace CozyFileService.Application.Features.ManageFiles.Queries.GetFileContent
{
    public class GetFileContentQueryHandler : IRequestHandler<GetFileContentQuery, FileContentViewModel>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<CreateFileCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public GetFileContentQueryHandler(IMapper mapper, 
            ILogger<CreateFileCommandHandler> logger, 
            IFileStorageService fileStorageService, 
            ILoggedInUserService loggedInUserService)
        {
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
            _loggedInUserService = loggedInUserService;
        }

        public async Task<FileContentViewModel> Handle(GetFileContentQuery request, CancellationToken cancellationToken)
        {
            var filename = request.FileName;    
            var containerName = _loggedInUserService.UserId;

            var isFileExists = await _fileStorageService.FileExistsAsync(containerName, filename);
            if (!isFileExists)
            {
                _logger.LogInformation($"Not found file name {request.FileName} in blob storage.");
                throw new NotFoundException(nameof(FileContentViewModel), filename);
            }

            var content = await _fileStorageService.DownloadFileAsync(containerName, filename);
            _logger.LogInformation($"Download file {request.FileName} from blob storage successfully.");

            var fileContent = new FileContentViewModel
            {
                FileName = request.FileName,
                ContentType = "application/octet-stream",
                Content = content
            };

            return fileContent;
        }
    }
}
