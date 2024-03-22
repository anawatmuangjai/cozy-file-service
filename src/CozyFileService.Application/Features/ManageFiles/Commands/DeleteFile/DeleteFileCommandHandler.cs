using AutoMapper;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Exceptions;
using CozyFileService.Domain.Entities;
using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRepository _uploadedFileRepository;

        public DeleteFileCommandHandler(IMapper mapper, IUploadedFileRepository uploadedFileRepository)
        {
            _mapper = mapper;
            _uploadedFileRepository = uploadedFileRepository;
        }

        public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var fileToDelete = await _uploadedFileRepository.GetByIdAsync(request.Id);

            if (fileToDelete == null)
            {
                throw new NotFoundException(nameof(UploadedFile), request.Id);
            }

            await _uploadedFileRepository.DeleteAsync(fileToDelete);
        }
    }
}
