using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile
{
    public class UpdateFileCommand : IRequest
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
    }
}
