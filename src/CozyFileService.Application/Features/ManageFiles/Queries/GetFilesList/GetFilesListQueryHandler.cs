using AutoMapper;
using CozyFileService.Application.Contracts.Persistence;
using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Queries.GetFilesList
{
    public class GetFilesListQueryHandler : IRequestHandler<GetFilesListQuery, List<FilesListViewModel>>
    {
        private readonly IMapper _mapper;
        private readonly IUploadedFileRepository _uploadedFileRepository;

        public GetFilesListQueryHandler(IMapper mapper, IUploadedFileRepository uploadedFileRepository)
        {
            _mapper = mapper;
            _uploadedFileRepository = uploadedFileRepository;
        }

        public async Task<List<FilesListViewModel>> Handle(GetFilesListQuery request, CancellationToken cancellationToken)
        {
            var allFiles = (await _uploadedFileRepository.GetAllAsync()).OrderBy(x => x.CreatedDate);
            return _mapper.Map<List<FilesListViewModel>>(allFiles);
        }
    }
}
