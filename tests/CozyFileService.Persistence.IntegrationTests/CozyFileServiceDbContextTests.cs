using CozyFileService.Application.Contracts;
using CozyFileService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CozyFileService.Persistence.IntegrationTests
{
    public class CozyFileServiceDbContextTests
    {
        private readonly CozyFileServiceDbContext _dbContext;
        private readonly Mock<ILoggedInUserService> _loggedInUserServiceMock;
        private readonly string _loggedInUserId;

        public CozyFileServiceDbContextTests()
        {
            _loggedInUserId = "TestUser";
            _loggedInUserServiceMock = new Mock<ILoggedInUserService>();
            _loggedInUserServiceMock.Setup(x => x.UserId).Returns(_loggedInUserId);

            var dbContextOptions = new DbContextOptionsBuilder<CozyFileServiceDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _dbContext = new CozyFileServiceDbContext(dbContextOptions, _loggedInUserServiceMock.Object);
        }

        [Fact]
        public async void Save_ValidFile_SetCreatedByProperty()
        {
            // Arrange
            var uploadedFile = new UploadedFile()
            {
                FileName = "TestFile",
                FileType = "txt",
                FilePath = "TestPath",
                FileSize = 123,
            };

            // Act
            _dbContext.UploadedFiles.Add(uploadedFile);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.Equal(_loggedInUserId, uploadedFile.CreatedBy);
        }   
    }
}
