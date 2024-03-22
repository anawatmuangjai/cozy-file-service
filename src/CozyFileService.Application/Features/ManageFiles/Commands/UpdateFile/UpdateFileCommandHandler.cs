using AutoMapper;
using CozyFileService.Application.Contracts.Persistance;
using CozyFileService.Domain.Entities;
using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile
{
    public class UpdateFileCommandHandler : IRequestHandler<UpdateFileCommand>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRespository _uploadedFileRepository;

        public UpdateFileCommandHandler(IMapper mapper, IUploadedFileRespository uploadedFileRepository)
        {
            _mapper = mapper;
            _uploadedFileRepository = uploadedFileRepository;
        }

        public async Task Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var fileToUpdate = await _uploadedFileRepository.GetByIdAsync(request.Id);

            _mapper.Map(request, fileToUpdate, typeof(UpdateFileCommand), typeof(UploadedFile));

            await _uploadedFileRepository.UpdateAsync(fileToUpdate);
        }
    }
}
