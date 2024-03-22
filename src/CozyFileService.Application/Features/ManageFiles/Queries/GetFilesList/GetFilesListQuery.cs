using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Queries.GetFilesList
{
    public class GetFilesListQuery : IRequest<List<FilesListViewModel>>
    {
    }
}
