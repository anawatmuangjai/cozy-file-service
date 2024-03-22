using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileCommand : IRequest<CreateFileCommandResponse>
    {
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentAsBase64String { get; set; } = string.Empty;
    }
}
