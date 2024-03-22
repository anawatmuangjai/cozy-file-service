using AutoMapper;
using CozyFileService.Application.Features.ManageFiles.Commands.CreateFile;
using CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile;
using CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile;
using CozyFileService.Application.Features.ManageFiles.Queries.GetFilesList;
using CozyFileService.Domain.Entities;

namespace CozyFileService.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();
            CreateMap<UploadedFile, FilesListViewModel>().ReverseMap();

            CreateMap<UploadedFile, CreateFileCommand>().ReverseMap();
            CreateMap<UploadedFile, UpdateFileCommand>().ReverseMap();
            CreateMap<UploadedFile, DeleteFileCommand>().ReverseMap();

            CreateMap<UploadedFile, CreateFileDto>().ReverseMap();
        }
    }
}
