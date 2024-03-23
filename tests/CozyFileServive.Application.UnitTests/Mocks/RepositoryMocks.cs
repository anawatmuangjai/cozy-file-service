using CozyFileService.Application.Contracts.Persistence;
using CozyFileService.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyFileServive.Application.UnitTests.Mocks
{
    public class RepositoryMocks
    {
        public static Mock<IUploadedFileRepository> GetUploadedFileRepository()
        {
            var testfileId1 = Guid.Parse("4F544ECD-19FB-4ADA-9E75-4A933B7C1B2F");
            var testfileId2 = Guid.Parse("7C416FB0-B9FA-43B3-81AB-EC3A3FCE8364");
            var testfileId3 = Guid.Parse("347D165F-1C89-40B4-A7E4-0E87BD2732D8");

            var filesList = new List<UploadedFile>
            {
                new UploadedFile
                {
                    Id = testfileId1,
                    FileName = "TestFile1",
                    FileType = ".txt",
                    FilePath = "TestPath1",
                    FileSize = 1024,
                },
                new UploadedFile
                {
                    Id = testfileId2,
                    FileName = "TestFile2",
                    FileType = ".pdf",
                    FilePath = "TestPath2",
                    FileSize = 2048,
                },
                new UploadedFile
                {
                    Id = testfileId3,
                    FileName = "TestFile3",
                    FileType = ".docx",
                    FilePath = "TestPath3",
                    FileSize = 4096,
                }
            };

            var mockUploadedFileRepository = new Mock<IUploadedFileRepository>();
            mockUploadedFileRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(filesList);
            mockUploadedFileRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(filesList[0]);
            mockUploadedFileRepository.Setup(r => r.AddAsync(It.IsAny<UploadedFile>())).ReturnsAsync(
                (UploadedFile file) =>
                {
                    filesList.Add(file);
                    return file;
                });

            return mockUploadedFileRepository;
        }
    }
}
