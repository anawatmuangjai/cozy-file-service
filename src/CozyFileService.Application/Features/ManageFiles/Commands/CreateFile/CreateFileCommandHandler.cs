using AutoMapper;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Models.Mail;
using CozyFileService.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, CreateFileCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRepository _uploadedFileRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreateFileCommandHandler> _logger;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILoggedInUserService _loggedInUserService;

        public CreateFileCommandHandler(IMapper mapper, 
            IUploadedFileRepository uploadedFileRepository, 
            IEmailService emailService, 
            ILogger<CreateFileCommandHandler> logger, 
            IFileStorageService fileStorageService, 
            ILoggedInUserService loggedInUserService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _uploadedFileRepository = uploadedFileRepository ?? throw new ArgumentNullException(nameof(uploadedFileRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
            _loggedInUserService = loggedInUserService ?? throw new ArgumentNullException(nameof(loggedInUserService));
        }

        public async Task<CreateFileCommandResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
        {
            var createFileCommandResponse = new CreateFileCommandResponse();
            var validator = new CreateFileCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.Errors.Any())
            {
                createFileCommandResponse.Success = false;
                createFileCommandResponse.ValidationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            }

            if (createFileCommandResponse.Success)
            {
                var uploadedFile = new UploadedFile
                {
                    FileName = request.FileName,
                    FileType = request.FileType,
                    FileSize = request.FileSize,
                };

                var path = await _fileStorageService.UploadFileAsync(_loggedInUserService.UserName, uploadedFile.FileName, request.ContentStream);
                _logger.LogInformation($"Uploaded file {uploadedFile.FileName} to blob storage successfully.");

                uploadedFile.FilePath = path;

                uploadedFile = await _uploadedFileRepository.AddAsync(uploadedFile);
                createFileCommandResponse.UploadedFile = _mapper.Map<CreateFileDto>(uploadedFile);
                _logger.LogInformation($"Created record id {uploadedFile.Id} in database successfully.");

                var email = new Email
                {
                    To = _loggedInUserService.Email,
                    Subject = "A new file has been uploaded",
                    Body = $"A new file has been uploaded: {request.FileName}"
                };

                try
                {
                    await _emailService.SendEmailAsync(email);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Email sending failed due to an error with the mail service: {ex.Message}");
                }
            }

            return createFileCommandResponse;
        }
    }
}
