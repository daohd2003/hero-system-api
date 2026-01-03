using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Helpers;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using Moq;
using Repositories;
using Services;
using Services.Common;
using Xunit;

namespace Tests
{
    public class MissionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMissionRepository> _mockMissionRepo;
        private readonly Mock<IHeroRepository> _mockHeroRepo;
        private readonly Mock<IServiceHelper> _mockServiceHelper;
        private readonly IMapper _mapper;
        private readonly MissionService _missionService;

        public MissionServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMissionRepo = new Mock<IMissionRepository>();
            _mockHeroRepo = new Mock<IHeroRepository>();
            _mockServiceHelper = new Mock<IServiceHelper>();

            _mockUnitOfWork.Setup(u => u.Missions).Returns(_mockMissionRepo.Object);
            _mockUnitOfWork.Setup(u => u.Heroes).Returns(_mockHeroRepo.Object);

            _missionService = new MissionService(
                _mockUnitOfWork.Object,
                _mockServiceHelper.Object,
                _mapper
            );
        }

        #region CreateMissionAsync Tests

        [Fact]
        public async Task CreateMissionAsync_Success_ReturnsCreatedMission()
        {
            var dto = new MissionDtos.CreateMissionDto { Title = "Save the World", DifficultyLevel = 10 };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockMissionRepo.Setup(r => r.AddAsync(It.IsAny<Mission>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _missionService.CreateMissionAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("Save the World", result.Data!.Title);
        }

        [Fact]
        public async Task CreateMissionAsync_Exception_ReturnsError()
        {
            var dto = new MissionDtos.CreateMissionDto { Title = "Test Mission", DifficultyLevel = 5 };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockMissionRepo.Setup(r => r.AddAsync(It.IsAny<Mission>())).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<MissionDtos.MissionDto>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<MissionDtos.MissionDto>.Error("Database error"));

            var result = await _missionService.CreateMissionAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion


        #region GetAllMissionsAsync Tests

        [Fact]
        public async Task GetAllMissionsAsync_Success_ReturnsMissionList()
        {
            var missions = new List<Mission>
            {
                new Mission { Id = Guid.NewGuid(), Title = "Mission 1", DifficultyLevel = 5 },
                new Mission { Id = Guid.NewGuid(), Title = "Mission 2", DifficultyLevel = 8 }
            };

            var mockQueryable = missions.AsQueryable().BuildMock();
            _mockMissionRepo.Setup(r => r.GetQueryable()).Returns(mockQueryable);

            var result = await _missionService.GetAllMissionsAsync();

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, result.Data!.Count);
        }

        [Fact]
        public async Task GetAllMissionsAsync_Exception_ReturnsError()
        {
            _mockMissionRepo.Setup(r => r.GetQueryable()).Throws(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<List<MissionDtos.MissionDto>>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<List<MissionDtos.MissionDto>>.Error("Database error"));

            var result = await _missionService.GetAllMissionsAsync();

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion

        #region AssignMissionToHeroAsync Tests

        [Fact]
        public async Task AssignMissionToHeroAsync_Success_ReturnsTrue()
        {
            var heroId = Guid.NewGuid();
            var missionId = Guid.NewGuid();
            var dto = new MissionDtos.AssignMissionDto { HeroId = heroId, MissionId = missionId, ResultRank = "S" };

            var hero = new Hero { Id = heroId, Name = "Test Hero" };
            var mission = new Mission { Id = missionId, Title = "Test Mission" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);
            _mockMissionRepo.Setup(r => r.GetByIdAsync(missionId)).ReturnsAsync(mission);
            _mockMissionRepo.Setup(r => r.GetHeroMissionAsync(heroId, missionId)).ReturnsAsync((HeroMission?)null);
            _mockMissionRepo.Setup(r => r.AddHeroMissionAsync(It.IsAny<HeroMission>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _missionService.AssignMissionToHeroAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task AssignMissionToHeroAsync_HeroNotFound_ReturnsNotFound()
        {
            var dto = new MissionDtos.AssignMissionDto { HeroId = Guid.NewGuid(), MissionId = Guid.NewGuid(), ResultRank = "A" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.GetByIdAsync(dto.HeroId)).ReturnsAsync((Hero?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.NotFound("Hero không tồn tại"));

            var result = await _missionService.AssignMissionToHeroAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task AssignMissionToHeroAsync_MissionNotFound_ReturnsNotFound()
        {
            var heroId = Guid.NewGuid();
            var missionId = Guid.NewGuid();
            var dto = new MissionDtos.AssignMissionDto { HeroId = heroId, MissionId = missionId, ResultRank = "A" };

            var hero = new Hero { Id = heroId, Name = "Test Hero" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);
            _mockMissionRepo.Setup(r => r.GetByIdAsync(missionId)).ReturnsAsync((Mission?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.NotFound("Mission không tồn tại"));

            var result = await _missionService.AssignMissionToHeroAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task AssignMissionToHeroAsync_AlreadyCompleted_ReturnsBadRequest()
        {
            var heroId = Guid.NewGuid();
            var missionId = Guid.NewGuid();
            var dto = new MissionDtos.AssignMissionDto { HeroId = heroId, MissionId = missionId, ResultRank = "A" };

            var hero = new Hero { Id = heroId, Name = "Test Hero" };
            var mission = new Mission { Id = missionId, Title = "Test Mission" };
            var existingRecord = new HeroMission { HeroId = heroId, MissionId = missionId };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);
            _mockMissionRepo.Setup(r => r.GetByIdAsync(missionId)).ReturnsAsync(mission);
            _mockMissionRepo.Setup(r => r.GetHeroMissionAsync(heroId, missionId)).ReturnsAsync(existingRecord);

            _mockServiceHelper.Setup(s => s.HandleBadRequest<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.BadRequest("Hero đã hoàn thành nhiệm vụ này rồi!"));

            var result = await _missionService.AssignMissionToHeroAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task AssignMissionToHeroAsync_Exception_ReturnsError()
        {
            var dto = new MissionDtos.AssignMissionDto { HeroId = Guid.NewGuid(), MissionId = Guid.NewGuid(), ResultRank = "A" };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.GetByIdAsync(dto.HeroId)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<bool>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.Error("Database error"));

            var result = await _missionService.AssignMissionToHeroAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion
    }
}
