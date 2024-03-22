using AutoMapper;
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
        private readonly IUploadedFileRepository _uploadedFileRespository;
        private readonly IEmailService _emailService;
        private readonly ILogger<CreateFileCommandHandler> _logger;

        public CreateFileCommandHandler(IMapper mapper, IUploadedFileRepository uploadedFileRespository, IEmailService emailService, ILogger<CreateFileCommandHandler> logger)
        {
            _mapper = mapper;
            _uploadedFileRespository = uploadedFileRespository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<CreateFileCommandResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
        {
            var createFileCommandResponse = new CreateFileCommandResponse();
            var validator = new CreateFileCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (validationResult.Errors.Count > 0)
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
                    FilePath = request.FilePath,
                    FileSize = request.FileSize,
                };

                uploadedFile = await _uploadedFileRespository.AddAsync(uploadedFile);
                createFileCommandResponse.UploadedFile = _mapper.Map<CreateFileDto>(uploadedFile);

                var email = new Email
                {
                    To = "anawat.mu@outlook.com",
                    Subject = "A new file has been uploaded",
                    Body = $"A new file has been uploaded: {request.FileName}"
                };

                try
                {
                    await _emailService.SendEmailAsync(email);
                }
                catch (Exception ex)
                {
                    // should't stop the API from doing other things if email fails
                    _logger.LogError($"Email sending failed due to an error with the mail service: {ex.Message}");
                }
            }

            return createFileCommandResponse;
        }
    }
}
