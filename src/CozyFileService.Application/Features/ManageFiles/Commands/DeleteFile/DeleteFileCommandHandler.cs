using AutoMapper;
using CozyFileService.Application.Contracts.Persistance;
using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRespository _uploadedFileRepository;

        public DeleteFileCommandHandler(IMapper mapper, IUploadedFileRespository uploadedFileRepository)
        {
            _mapper = mapper;
            _uploadedFileRepository = uploadedFileRepository;
        }

        public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var fileToDelete = await _uploadedFileRepository.GetByIdAsync(request.Id);

            await _uploadedFileRepository.DeleteAsync(fileToDelete);
        }
    }
}
