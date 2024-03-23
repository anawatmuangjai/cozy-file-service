using AutoMapper;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Features.ManageFiles.Commands.CreateFile;
using CozyFileService.Application.Models.Mail;
using CozyFileService.Application.Profiles;
using CozyFileService.Domain.Entities;
using CozyFileServive.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace CozyFileServive.Application.UnitTests.ManageFiles.Commands
{
    public class CreateFileCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUploadedFileRepository> _mockUploadedFileRepository;
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ILogger<CreateFileCommandHandler>> _mockLogger;
        private readonly Mock<ILoggedInUserService> _mockLoggedInUserService;

        public CreateFileCommandHandlerTests() 
        {
            _mockLogger = new Mock<ILogger<CreateFileCommandHandler>>();

            _mockUploadedFileRepository = RepositoryMocks.GetUploadedFileRepository();
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockLoggedInUserService = new Mock<ILoggedInUserService>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_ValidCreateFileCommandRequest_Success()
        {
            // Arrange
            var createFileCommand = new CreateFileCommand()
            {
                FileName = "TestFile",
                FileType = "txt",
                FileSize = 123,
                ContentStream = new MemoryStream(new byte[1024])
            };

            _mockLoggedInUserService.Setup(u => u.UserName).Returns("testuser");
            _mockLoggedInUserService.Setup(u => u.Email).Returns("test@example.com");

            _mockFileStorageService
                .Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .ReturnsAsync("https://example.com/test.txt");

            _mockEmailService.Setup(e => e.SendEmailAsync(It.IsAny<Email>()))
                .ReturnsAsync(true);

            var handler = new CreateFileCommandHandler(_mapper,
                _mockUploadedFileRepository.Object,
                _mockEmailService.Object,
                _mockLogger.Object,
                _mockFileStorageService.Object,
                _mockLoggedInUserService.Object);

            // Act
            var response = await handler.Handle(createFileCommand, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Null(response.ValidationErrors);

            _mockFileStorageService.Verify(s => s.UploadFileAsync("testuser", "TestFile", createFileCommand.ContentStream), Times.Once);
            _mockUploadedFileRepository.Verify(r => r.AddAsync(It.IsAny<UploadedFile>()), Times.Once);
            _mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<Email>()), Times.Once);
            
            var totalFiles = await _mockUploadedFileRepository.Object.GetAllAsync();
            Assert.Equal(4, totalFiles.Count);
        }
    }
}
