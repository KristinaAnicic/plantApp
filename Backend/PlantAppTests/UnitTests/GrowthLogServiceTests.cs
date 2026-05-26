using Microsoft.Extensions.Logging;
using Moq;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Interfaces;
using PlantApp.Domain.Services.Data;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantAppTests.UnitTests
{
    [TestFixture]
    public class GrowthLogServiceTests
    {
        private Mock<IGrowthLogRepository> _repositoryMock;
        private Mock<IRepository<PlantStatus>> _statusRepoMock;
        private Mock<IPlantedRepository> _plantedRepoMock;
        private Mock<IPlantGroupRepository> _groupRepoMock;
        private Mock<IImageService> _imageServiceMock;
        private Mock<ICurrentUserContext> _userContextMock;
        private Mock<ILogger<GrowthLogService>> _loggerMock;
        private GrowthLogService _service;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IGrowthLogRepository>();
            _statusRepoMock = new Mock<IRepository<PlantStatus>>();
            _plantedRepoMock = new Mock<IPlantedRepository>();
            _groupRepoMock = new Mock<IPlantGroupRepository>();
            _imageServiceMock = new Mock<IImageService>();
            _userContextMock = new Mock<ICurrentUserContext>();
            _loggerMock = new Mock<ILogger<GrowthLogService>>();

            _userContextMock.Setup(u => u.GetCurrentUserId()).Returns(1);
            _userContextMock.Setup(u => u.GetCurrentUserRoleId()).Returns(2); // not admin

            _service = new GrowthLogService(
                _repositoryMock.Object,
                _statusRepoMock.Object,
                _plantedRepoMock.Object,
                _groupRepoMock.Object,
                _imageServiceMock.Object,
                _userContextMock.Object,
                _loggerMock.Object
            );
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedGrowthLogs()
        {
            var logs = new List<GrowthLog>
            {
                new GrowthLog { Id = 1, Title = "Title 1" },
                new GrowthLog { Id = 2, Title = "Title 2" }
            };
            _repositoryMock.Setup(r => r.GetAllGrowthLogsByUserId(1)).ReturnsAsync(logs);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            _repositoryMock.Verify(r => r.GetAllGrowthLogsByUserId(1), Times.Once);
        }

        [Test]
        public async Task GetAllByPlantedIdAsync_ReturnsEmpty_WhenPlantedNotFound()
        {
            _plantedRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync((Planted)null);

            var result = await _service.GetAllByPlantedIdAsync(1);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllByPlantedIdAsync_ThrowsUnauthorized_WhenUserNotOwner()
        {
            var planted = new Planted
            {
                Id = 1,
                PlantGroupId = null,
                Place = new Place { UserId = 2, Name = "Place 1" }, // different user
                PlantId = 1,
                PlaceId = 1,
                DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow),
            };
            var logs = new List<GrowthLog> { new GrowthLog { Id = 10, Title = "Title" } };

            _plantedRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(planted);
            _repositoryMock.Setup(r => r.GetAllGrowthLogsByPlantedId(1, null)).ReturnsAsync(logs);

            Assert.That(async () => await _service.GetAllByPlantedIdAsync(1),
                Throws.TypeOf<UnauthorizedException>());
        }

        [Test]
        public void GetByIdAsync_ThrowsNotFound_WhenLogNotExists()
        {
            _repositoryMock.Setup(r => r.GetGrowthLogById(1)).ReturnsAsync((GrowthLog)null);

            Assert.That(async () => await _service.GetByIdAsync(1),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        public void AddAsync_Throws_WhenNoPlantedOrGroupId()
        {
            var dto = new UpsertGrowthLogDto
            {
                PlantGroupId = null,
                PlantedId = null,
                PlantStatusId = 1,
                Title = "Title"
            };

            Assert.That(async () => await _service.AddAsync(dto),
                Throws.TypeOf<InvalidOperationAppException>());
        }

        [Test]
        public async Task AddAsync_AddsLog_WhenValidPlantedDto()
        {
            var dto = new UpsertGrowthLogDto
            {
                PlantedId = 1,
                PlantStatusId = 1,
                Images = new List<string>(),
                Title = "Title"
            };

            var planted = new Planted
            {
                Id = 1,
                PlaceId = 10,
                Place = new Place { UserId = 1, Name = "Place 1" },
                PlantId = 1,
                DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow),
            };
            _plantedRepoMock.Setup(p => p.GetByIdAsync(1)).ReturnsAsync(planted);
            _statusRepoMock.Setup(s => s.IdExistsAsync(1)).ReturnsAsync(true);

            await _service.AddAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<GrowthLog>()), Times.Once);
        }

        [Test]
        public void UpdateAsync_ThrowsDtoIdMismatch()
        {
            var dto = new UpsertGrowthLogDto { 
                Id = 2, 
                PlantedId = 1, 
                PlantStatusId = 1,
                Title = "Title"
            };
            Assert.That(async () => await _service.UpdateAsync(1, dto),
                Throws.TypeOf<DtoIdMismatchException>());
        }

        [Test]
        public async Task DeleteAsync_DeletesLog_WhenAuthorized()
        {
            var log = new GrowthLog
            {
                Id = 1,
                Title = "Title",
                Planted = new List<Planted> { 
                    new Planted { 
                        Id = 1,
                        PlaceId = 10,
                        Place = new Place { UserId = 1, Name = "Place 1" },
                        PlantId = 1,
                        DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow),
                    } 
                }
            };
            _repositoryMock.Setup(r => r.GetGrowthLogById(1)).ReturnsAsync(log);

            await _service.DeleteAsync(1);

            _repositoryMock.Verify(r => r.DeleteGrowthLog(log), Times.Once);
        }

        [Test]
        public void DeleteAsync_ThrowsUnauthorized_WhenNotAuthorized()
        {
            var log = new GrowthLog
            {
                Id = 1,
                Title = "Title",
                Planted = new List<Planted> { 
                    new Planted {
                        Id = 1,
                        PlaceId = 10,
                        Place = new Place { UserId = 2, Name = "Place 1" } ,
                        PlantId = 1,
                        DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow),
                    } 
                }
            };
            _repositoryMock.Setup(r => r.GetGrowthLogById(1)).ReturnsAsync(log);

            Assert.That(async () => await _service.DeleteAsync(1),
                Throws.TypeOf<UnauthorizedException>());
        }

        [Test]
        public async Task AddImages_CallsImageServiceAndRepository()
        {
            var log = new GrowthLog
            {
                Id = 1,
                Title = "Title",
                Planted = new List<Planted> { 
                    new Planted {
                        Id = 1,
                        PlaceId = 10,
                        Place = new Place { UserId = 1, Name = "Place 1" },
                        PlantId = 1,
                        DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow),
                    } 
                }
            };
            _repositoryMock.Setup(r => r.GetGrowthLogById(1)).ReturnsAsync(log);

            await _service.AddImages(1, new List<string> { "url1", "url2" });

            _imageServiceMock.Verify(i => i.AddImagesToEntityAsync(log, It.IsAny<List<string>>()), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(log), Times.Once);
        }

        [Test]
        public async Task RemoveImageById_CallsImageService()
        {
            var log = new GrowthLog
            {
                Id = 1,
                Title = "Title",
                Planted = new List<Planted> { 
                    new Planted {
                        Id = 1,
                        PlaceId = 10,
                        Place = new Place { UserId = 1, Name = "Place 1" },
                        PlantId = 1,
                        DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow)
                    } 
                }
            };
            _repositoryMock.Setup(r => r.GetGrowthLogById(1)).ReturnsAsync(log);

            await _service.RemoveImageById(1, 5);

            _imageServiceMock.Verify(i => i.RemoveImageFromEntityAsync(log, 5, _repositoryMock.Object), Times.Once);
        }

    }
}
