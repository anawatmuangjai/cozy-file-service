using AutoMapper;
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
        }
    }
}
