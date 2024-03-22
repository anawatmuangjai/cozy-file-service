using CozyFileService.Application.Response;

namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileCommandResponse : BaseResponse
    {
        public CreateFileCommandResponse() : base()
        {
        }

        public CreateFileDto UploadedFile { get; set; } = default!;
    }
}
