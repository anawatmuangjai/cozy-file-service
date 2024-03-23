using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Queries.GetFileContent
{
    public class GetFileContentQuery : IRequest<FileContentViewModel>
    {
        public string FileName { get; set; }
    }
}
