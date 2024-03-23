using AutoMapper;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Features.ManageFiles.Queries.GetFilesList;
using CozyFileService.Application.Profiles;
using CozyFileServive.Application.UnitTests.Mocks;
using Moq;

namespace CozyFileServive.Application.UnitTests.ManageFiles.Queries
{
    public class GetFilesListQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUploadedFileRepository> _mockUploadedFileRepository;

        public GetFilesListQueryHandlerTests()
        {
            _mockUploadedFileRepository = RepositoryMocks.GetUploadedFileRepository();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_GetFilesListQuery_ReturnFileListWithExpectedResult()
        {
            // Arrange
            var handler = new GetFilesListQueryHandler(_mapper, _mockUploadedFileRepository.Object);

            // Act
            var result = await handler.Handle(new GetFilesListQuery(), CancellationToken.None);

            // Assert
            _mockUploadedFileRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
            Assert.IsType<List<FilesListViewModel>>(result);
            Assert.NotEmpty(result);
            Assert.Equal(3, result.Count);
        }
    }
}
