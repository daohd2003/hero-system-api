using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Helpers;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Repositories;
using Services;
using Services.Common;
using Xunit;

namespace Tests
{
    public class FactionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IFactionRepository> _mockFactionRepo;
        private readonly Mock<IServiceHelper> _mockServiceHelper;
        private readonly IMapper _mapper;
        private readonly FactionService _factionService;

        public FactionServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockFactionRepo = new Mock<IFactionRepository>();
            _mockServiceHelper = new Mock<IServiceHelper>();

            _mockUnitOfWork.Setup(u => u.Factions).Returns(_mockFactionRepo.Object);

            _factionService = new FactionService(
                _mockUnitOfWork.Object,
                _mapper,
                _mockServiceHelper.Object
            );
        }

        #region CreateFactionAsync Tests

        [Fact]
        public async Task CreateFactionAsync_Success_ReturnsCreatedFaction()
        {
            var dto = new FactionDtos.CreateFactionDto { Name = "Avengers", Description = "Earth's Mightiest Heroes" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockFactionRepo.Setup(r => r.AddAsync(It.IsAny<Faction>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _factionService.CreateFactionAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("Avengers", result.Data!.Name);
        }

        [Fact]
        public async Task CreateFactionAsync_Exception_ReturnsError()
        {
            var dto = new FactionDtos.CreateFactionDto { Name = "Test Faction", Description = "Test" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockFactionRepo.Setup(r => r.AddAsync(It.IsAny<Faction>())).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<FactionDtos.FactionDto>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<FactionDtos.FactionDto>.Error("Database error"));

            var result = await _factionService.CreateFactionAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion


        #region GetAllFactionsAsync Tests

        [Fact]
        public async Task GetAllFactionsAsync_Success_ReturnsFactionList()
        {
            var factions = new List<Faction>
            {
                new Faction { Id = Guid.NewGuid(), Name = "Avengers", Heroes = new List<Hero>() },
                new Faction { Id = Guid.NewGuid(), Name = "X-Men", Heroes = new List<Hero>() }
            };

            _mockFactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(factions);

            var result = await _factionService.GetAllFactionsAsync();

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, result.Data!.Count);
        }

        [Fact]
        public async Task GetAllFactionsAsync_EmptyList_ReturnsEmptyList()
        {
            _mockFactionRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Faction>());

            var result = await _factionService.GetAllFactionsAsync();

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Empty(result.Data!);
        }

        [Fact]
        public async Task GetAllFactionsAsync_Exception_ReturnsError()
        {
            _mockFactionRepo.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<List<FactionDtos.FactionDto>>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<List<FactionDtos.FactionDto>>.Error("Database error"));

            var result = await _factionService.GetAllFactionsAsync();

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion

        #region DeleteFactionAsync Tests

        [Fact]
        public async Task DeleteFactionAsync_FactionExists_ReturnsSuccess()
        {
            var factionId = Guid.NewGuid();
            var faction = new Faction { Id = factionId, Name = "Test Faction" };

            _mockFactionRepo.Setup(r => r.GetByIdAsync(factionId)).ReturnsAsync(faction);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _factionService.DeleteFactionAsync(factionId);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _mockFactionRepo.Verify(r => r.DeleteAsync(faction), Times.Once);
        }

        [Fact]
        public async Task DeleteFactionAsync_FactionNotFound_ReturnsNotFound()
        {
            var factionId = Guid.NewGuid();
            _mockFactionRepo.Setup(r => r.GetByIdAsync(factionId)).ReturnsAsync((Faction?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.NotFound("Faction not found"));

            var result = await _factionService.DeleteFactionAsync(factionId);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task DeleteFactionAsync_Exception_ReturnsError()
        {
            var factionId = Guid.NewGuid();
            _mockFactionRepo.Setup(r => r.GetByIdAsync(factionId)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<bool>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.Error("Database error"));

            var result = await _factionService.DeleteFactionAsync(factionId);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion
    }
}
