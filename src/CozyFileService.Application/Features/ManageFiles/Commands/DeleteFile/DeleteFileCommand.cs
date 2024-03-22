using MediatR;

namespace CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile
{
    public class DeleteFileCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
