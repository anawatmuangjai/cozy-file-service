namespace CozyFileService.Application.Features.ManageFiles.Commands.CreateFile
{
    public class CreateFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } 
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
    }
}
