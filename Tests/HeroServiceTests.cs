using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Helpers;
using BusinessObject.Models;
using Moq;
using Repositories;
using Services;
using Services.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Tests
{
    public class HeroServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IHeroRepository> _mockHeroRepo;
        private readonly Mock<IServiceHelper> _mockServiceHelper;
        private readonly IMapper _mapper;
        private readonly HeroService _heroService;

        public HeroServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();

            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHeroRepo = new Mock<IHeroRepository>();
            _mockServiceHelper = new Mock<IServiceHelper>();

            _mockUnitOfWork.Setup(u => u.Heroes).Returns(_mockHeroRepo.Object);

            _heroService = new HeroService(_mapper, _mockServiceHelper.Object, _mockUnitOfWork.Object);
        }

        #region CreateHeroAsync Tests

        [Fact]
        public async Task CreateHeroAsync_Success_ReturnsCreatedHero()
        {
            var dto = new CreateHeroDto { Name = "Iron Man", SuperPower = "Money", Level = 99 };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.AddAsync(It.IsAny<Hero>())).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _heroService.CreateHeroAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(201, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Iron Man", result.Data.Name);
        }

        [Fact]
        public async Task CreateHeroAsync_Exception_ReturnsError()
        {
            var dto = new CreateHeroDto { Name = "Test Hero", SuperPower = "Test", Level = 1 };

            var mockTransaction = new Mock<IDbContextTransaction>();
            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            _mockHeroRepo.Setup(r => r.AddAsync(It.IsAny<Hero>())).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<Hero>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<Hero>.Error("Database error"));

            var result = await _heroService.CreateHeroAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion


        #region GetHeroByIdAsync Tests

        [Fact]
        public async Task GetHeroByIdAsync_HeroExists_ReturnsHero()
        {
            var heroId = Guid.NewGuid();
            var hero = new Hero { Id = heroId, Name = "Spider Man", SuperPower = "Web", Level = 80 };

            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);

            var result = await _heroService.GetHeroByIdAsync(heroId);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("Spider Man", result.Data!.Name);
        }

        [Fact]
        public async Task GetHeroByIdAsync_HeroNotFound_ReturnsNotFound()
        {
            var heroId = Guid.NewGuid();
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync((Hero?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<HeroDto?>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<HeroDto?>.NotFound("Hero không tồn tại"));

            var result = await _heroService.GetHeroByIdAsync(heroId);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task GetHeroByIdAsync_Exception_ReturnsError()
        {
            var heroId = Guid.NewGuid();
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<HeroDto?>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<HeroDto?>.Error("Database error"));

            var result = await _heroService.GetHeroByIdAsync(heroId);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion

        #region DeleteHeroAsync Tests

        [Fact]
        public async Task DeleteHeroAsync_HeroExists_ReturnsSuccess()
        {
            var heroId = Guid.NewGuid();
            var hero = new Hero { Id = heroId, Name = "Test Hero" };

            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _heroService.DeleteHeroAsync(heroId);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _mockHeroRepo.Verify(r => r.Delete(hero), Times.Once);
        }

        [Fact]
        public async Task DeleteHeroAsync_HeroNotFound_ReturnsNotFound()
        {
            var heroId = Guid.NewGuid();
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync((Hero?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.NotFound("Hero không tồn tại"));

            var result = await _heroService.DeleteHeroAsync(heroId);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task DeleteHeroAsync_Exception_ReturnsError()
        {
            var heroId = Guid.NewGuid();
            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<bool>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.Error("Database error"));

            var result = await _heroService.DeleteHeroAsync(heroId);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion

        #region UpdateHeroAsync Tests

        [Fact]
        public async Task UpdateHeroAsync_Success_ReturnsTrue()
        {
            var heroId = Guid.NewGuid();
            var hero = new Hero { Id = heroId, Name = "Old Name", SuperPower = "Old Power", Level = 10 };
            var dto = new UpdateHeroDto { Name = "New Name", SuperPower = "New Power", Level = 50 };

            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync(hero);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _heroService.UpdateHeroAsync(heroId, dto);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            _mockHeroRepo.Verify(r => r.Update(It.IsAny<Hero>()), Times.Once);
        }

        [Fact]
        public async Task UpdateHeroAsync_HeroNotFound_ReturnsNotFound()
        {
            var heroId = Guid.NewGuid();
            var dto = new UpdateHeroDto { Name = "New Name", SuperPower = "New Power", Level = 50 };

            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ReturnsAsync((Hero?)null);

            _mockServiceHelper.Setup(s => s.HandleNotFound<bool>(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.NotFound("Hero không tồn tại"));

            var result = await _heroService.UpdateHeroAsync(heroId, dto);

            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task UpdateHeroAsync_Exception_ReturnsError()
        {
            var heroId = Guid.NewGuid();
            var dto = new UpdateHeroDto { Name = "New Name", SuperPower = "New Power", Level = 50 };

            _mockHeroRepo.Setup(r => r.GetByIdAsync(heroId)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<bool>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<bool>.Error("Database error"));

            var result = await _heroService.UpdateHeroAsync(heroId, dto);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion

        #region GetAllHerosAsync Tests

        [Fact]
        public async Task GetAllHerosAsync_Success_ReturnsPagedResult()
        {
            var heroes = new List<Hero>
            {
                new Hero { Id = Guid.NewGuid(), Name = "Hero 1", SuperPower = "Power 1", Level = 50 },
                new Hero { Id = Guid.NewGuid(), Name = "Hero 2", SuperPower = "Power 2", Level = 60 }
            };

            _mockHeroRepo.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync((heroes, 2));

            var result = await _heroService.GetAllHerosAsync(1, 10);

            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(2, result.Data!.TotalCount);
        }

        [Fact]
        public async Task GetAllHerosAsync_Exception_ReturnsError()
        {
            _mockHeroRepo.Setup(r => r.GetAllAsync(1, 10)).ThrowsAsync(new Exception("Database error"));

            _mockServiceHelper.Setup(s => s.HandleError<PagedResult<HeroDto>>(It.IsAny<Exception>(), It.IsAny<string>()))
                .Returns(ServiceResult<PagedResult<HeroDto>>.Error("Database error"));

            var result = await _heroService.GetAllHerosAsync(1, 10);

            Assert.False(result.Success);
            Assert.Equal(500, result.StatusCode);
        }

        #endregion
    }
}
