using AutoMapper;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Contracts;
using Microsoft.Extensions.Logging;
using Moq;
using CozyFileService.Application.Exceptions;
using CozyFileService.Application.Features.ManageFiles.Commands.UpdateFile;
using CozyFileService.Domain.Entities;
using CozyFileService.Application.Profiles;
using CozyFileServive.Application.UnitTests.Mocks;

namespace CozyFileServive.Application.UnitTests.ManageFiles.Commands
{
    public class UpdateFileCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUploadedFileRepository> _mockUploadedFileRepository;
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<ILogger<UpdateFileCommandHandler>> _mockLogger;
        private readonly Mock<ILoggedInUserService> _mockLoggedInUserService;

        public UpdateFileCommandHandlerTests()
        {
            _mockLogger = new Mock<ILogger<UpdateFileCommandHandler>>();

            _mockUploadedFileRepository = RepositoryMocks.GetUploadedFileRepository();
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockLoggedInUserService = new Mock<ILoggedInUserService>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_ValidUpdateFileCommandRequest_Success()
        {
            // Arrange
            var command = new UpdateFileCommand
            {
                Id = Guid.Parse("4F544ECD-19FB-4ADA-9E75-4A933B7C1B2F"),
                FileName = "TestRenameFile.txt",
            };

            _mockLoggedInUserService.Setup(u => u.UserId).Returns("userId");
            _mockLoggedInUserService.Setup(u => u.Email).Returns("test@example.com");

            _mockFileStorageService
                .Setup(s => s.FileExistsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockFileStorageService
                .Setup(s => s.UpdateFileNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("https://example.com/TestRenameFile.txt");

            _mockUploadedFileRepository
                .Setup(repo => repo.UpdateAsync(It.IsAny<UploadedFile>()))
                .Returns(Task.CompletedTask);

            var handler = new UpdateFileCommandHandler(_mapper,
                _mockLogger.Object,
                _mockUploadedFileRepository.Object,
                _mockFileStorageService.Object,
                _mockLoggedInUserService.Object);

            await handler.Handle(command, CancellationToken.None);

            // Assert
            _mockFileStorageService.Verify(s => s.FileExistsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockFileStorageService.Verify(s => s.UpdateFileNameAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUploadedFileRepository.Verify(repo => repo.UpdateAsync(It.IsAny<UploadedFile>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenFileToUpdateDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var command = new UpdateFileCommand 
            { 
                Id = Guid.Parse("4F544ECD-19FB-4ADA-9E75-4A933B7C1B2F"),
                FileName = "TestRenameFile.txt"
            };

            _mockUploadedFileRepository
                .Setup(repo => repo.GetByIdAsync(command.Id))
                .Returns(Task.FromResult<UploadedFile>(null));

            var handler = new UpdateFileCommandHandler(_mapper, 
                _mockLogger.Object, 
                _mockUploadedFileRepository.Object, 
                _mockFileStorageService.Object, 
                _mockLoggedInUserService.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenValidationFails_ThrowsValidationException()
        {
            // Arrange
            var command = new UpdateFileCommand();

            var validationResult = new FluentValidation.Results.ValidationResult();
            validationResult.Errors.
                Add(new FluentValidation.Results.ValidationFailure("FileName", "FileName is required"));

            _mockUploadedFileRepository
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new UploadedFile());
            
            var handler = new UpdateFileCommandHandler(_mapper,
                _mockLogger.Object,
                _mockUploadedFileRepository.Object,
                _mockFileStorageService.Object,
                _mockLoggedInUserService.Object);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => handler.Handle(command, CancellationToken.None));
            var count = ex.ValidationErrors.Count;
            Assert.Equal(1, count);
        }


    }
}
