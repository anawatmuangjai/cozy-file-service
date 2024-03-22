using AutoMapper;
using CozyFileService.Application.Contracts.Persistance;
using CozyFileService.Domain.Entities;
using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileCommandHandler : IRequestHandler<CreateFileCommand, CreateFileCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRespository _uploadedFileRespository;

        public CreateFileCommandHandler(IMapper mapper, IUploadedFileRespository uploadedFileRespository)
        {
            _mapper = mapper;
            _uploadedFileRespository = uploadedFileRespository;
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
            }

            return createFileCommandResponse;
        }
    }
}
