using AutoMapper;
using CozyFileService.Application.Contracts;
using CozyFileService.Application.Contracts.Infrastructure;
using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Application.Exceptions;
using CozyFileService.Application.Features.ManageFiles.Commands.DeleteFile;
using CozyFileService.Application.Profiles;
using CozyFileService.Domain.Entities;
using CozyFileServive.Application.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using Moq;

namespace CozyFileService.UnitTests.Application.Features.ManageFiles.Commands
{
    public class DeleteFileCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IUploadedFileRepository> _mockUploadedFileRepository;
        private readonly Mock<IFileStorageService> _mockFileStorageService;
        private readonly Mock<ILoggedInUserService> _mockLoggedInUserService;
        private readonly Mock<ILogger<DeleteFileCommandHandler>> _mockLogger;

        public DeleteFileCommandHandlerTests()
        {
            _mockUploadedFileRepository = RepositoryMocks.GetUploadedFileRepository();
            _mockFileStorageService = new Mock<IFileStorageService>();
            _mockLoggedInUserService = new Mock<ILoggedInUserService>();
            _mockLogger = new Mock<ILogger<DeleteFileCommandHandler>>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_WhenFileToDeleteExists_DeletesFileAndRemovesFromDatabase()
        {
            // Arrange
            var fileId = Guid.Parse("4F544ECD-19FB-4ADA-9E75-4A933B7C1B2F");
            var fileToDelete = new UploadedFile 
            { 
                Id = fileId, 
                FileName = "TestFile.txt" 
            };

            _mockUploadedFileRepository
                .Setup(repo => repo.GetByIdAsync(fileId))
                .ReturnsAsync(fileToDelete);

            _mockLoggedInUserService.Setup(u => u.UserId).Returns("userId");

            _mockFileStorageService
                .Setup(s => s.FileExistsAsync("userId", fileToDelete.FileName))
                .ReturnsAsync(true);

            _mockFileStorageService
                .Setup(s => s.DeleteFileAsync("userId", fileToDelete.FileName))
                .ReturnsAsync(true);

            var handler = new DeleteFileCommandHandler(_mapper, 
                _mockLogger.Object, 
                _mockUploadedFileRepository.Object, 
                _mockFileStorageService.Object, 
                _mockLoggedInUserService.Object);

            // Act
            await handler.Handle(new DeleteFileCommand { Id = fileId }, CancellationToken.None);

            // Assert
            _mockFileStorageService.Verify(repo => repo.FileExistsAsync("userId", fileToDelete.FileName), Times.Once);
            _mockFileStorageService.Verify(repo => repo.DeleteFileAsync("userId", fileToDelete.FileName), Times.Once);
            _mockUploadedFileRepository.Verify(repo => repo.DeleteAsync(fileToDelete), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenFileToDeleteDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var fileId = Guid.Parse("4F544ECD-19FB-4ADA-9E75-4A933B7C1B2F");

            _mockUploadedFileRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult<UploadedFile>(null));

            var handler = new DeleteFileCommandHandler(_mapper, 
                _mockLogger.Object, 
                _mockUploadedFileRepository.Object, 
                _mockFileStorageService.Object, 
                _mockLoggedInUserService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new DeleteFileCommand { Id = fileId }, CancellationToken.None));
        }
    }
}