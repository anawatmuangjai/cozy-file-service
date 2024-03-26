using AutoMapper;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Exceptions;
using CozyFileService.Application.Features.ManageFiles.Commands.CreateFile;
using CozyFileService.Application.Features.ManageFiles.Queries.GetFileContent;
using CozyFileService.Application.Profiles;
using Microsoft.Extensions.Logging;
using Moq;

namespace CozyFileService.UnitTests.Application.Features.ManageFiles.Queries
{
    public class GetFileContentQueryHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<ILoggedInUserService> _mockLoggedInUserService;
        private readonly Mock<ILogger<CreateFileCommandHandler>> _mockLogger;

        public GetFileContentQueryHandlerTests()
        {
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockLoggedInUserService = new Mock<ILoggedInUserService>();
            _mockLogger = new Mock<ILogger<CreateFileCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_WhenFileExists_ReturnsFileContentViewModel()
        {
            // Arrange
            var fileName = "test.txt";
            var containerName = "userId";
            var fileContent = new MemoryStream();

            _mockLoggedInUserService.Setup(u => u.UserId).Returns("userId");

            _mockFileStorageService
                .Setup(s => s.FileExistsAsync(containerName, fileName))
                .ReturnsAsync(true);

            _mockFileStorageService
                .Setup(s => s.DownloadFileAsync(containerName, fileName))
                .ReturnsAsync(fileContent);

            var handler = new GetFileContentQueryHandler(_mapper, 
                _mockLogger.Object, 
                _mockFileStorageService.Object, 
                _mockLoggedInUserService.Object);

            // Act
            var result = await handler.Handle(new GetFileContentQuery { FileName = fileName }, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal("application/octet-stream", result.ContentType);
            Assert.Equal(fileContent, result.Content);
        }

        [Fact]
        public async Task Handle_WhenFileDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var fileName = "nonexistent.txt";
            var containerName = "userId";

            _mockLoggedInUserService.Setup(u => u.UserId).Returns("userId");

            _mockFileStorageService
                .Setup(s => s.FileExistsAsync(containerName, fileName))
                .ReturnsAsync(false);

            var handler = new GetFileContentQueryHandler(_mapper, 
                _mockLogger.Object, 
                _mockFileStorageService.Object, 
                _mockLoggedInUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetFileContentQuery { FileName = fileName }, CancellationToken.None));
        }
    }
}
