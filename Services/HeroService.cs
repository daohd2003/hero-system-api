using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using Services.Common;

namespace Services
{
    public class HeroService : IHeroService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _serviceHelper;
        private readonly IHeroRepository _heroRepository;

        public HeroService(IMapper mapper, ServiceHelper serviceHelper, IHeroRepository heroRepository)
        {
            _mapper = mapper;
            _serviceHelper = serviceHelper;
            _heroRepository = heroRepository;
        }

        public async Task<ServiceResult<Hero>> CreateHeroAsync(CreateHeroDto dto)
        {
            Guid? heroId = null;
            await using var transaction = await _heroRepository.BeginTransactionAsync();
            try
            {
                var hero = _mapper.Map<Hero>(dto);
                hero.Id = Guid.NewGuid();
                heroId = hero.Id;
                await _heroRepository.AddAsync(hero);
                await _heroRepository.SaveChangesAsync();
                await transaction.CommitAsync();
                return ServiceResult<Hero>.Created(hero);
            }
            catch (Exception ex)
            {
                // Khi nhảy vào đây, transaction chưa được Commit
                // Khi return xong, "await using" sẽ tự động Dispose và Rollback
                // Không cần gọi Rollback thủ công
                return _serviceHelper.HandleError<Hero>(ex, $"Create Hero | Id: {heroId}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteHeroAsync(Guid id)
        {
            try
            {
                var hero = await _heroRepository.GetByIdAsync(id);
                if (hero == null)
                {
                    return _serviceHelper.HandleNotFound<bool>("Hero không tồn tại", $"Delete Hero | Id: {id}");
                }
                _heroRepository.Delete(hero);
                await _heroRepository.SaveChangesAsync();
                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<bool>(ex, $"Delete Hero | Id: {id}");
            }
        }

        public async Task<ServiceResult<PagedResult<HeroDto>>> GetAllHerosAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (heroes, totalCount) = await _heroRepository.GetAllAsync(pageNumber, pageSize);
                var heroDtos = _mapper.Map<List<HeroDto>>(heroes);
                var result = new PagedResult<HeroDto>
                {
                    Items = heroDtos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return ServiceResult<PagedResult<HeroDto>>.Ok(result);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<PagedResult<HeroDto>>(ex, $"GetAll Heroes | Page: {pageNumber}, Size: {pageSize}");
            }
        }

        public async Task<ServiceResult<HeroDto?>> GetHeroByIdAsync(Guid id)
        {
            try
            {
                var hero = await _heroRepository.GetByIdAsync(id);
                if (hero == null)
                    return _serviceHelper.HandleNotFound<HeroDto?>("Hero không tồn tại", $"GetById Hero | Id: {id}");
                var heroDto = _mapper.Map<HeroDto>(hero);
                return ServiceResult<HeroDto?>.Ok(heroDto);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<HeroDto?>(ex, $"GetById Hero | Id: {id}");
            }
        }

        public async Task<ServiceResult<HeroDto?>> GetHeroFullInfoAsync(Guid id)
        {
            var hero = await _heroRepository.GetHeroFullInfoAsync(id);
            if (hero == null)
                return _serviceHelper.HandleNotFound<HeroDto?>("Hero không tồn tại", $"GetById Hero | Id: {id}");
            var heroDto = _mapper.Map<HeroDto?>(hero);
            return ServiceResult<HeroDto?>.Ok(heroDto);
        }

        public async Task<ServiceResult<bool>> UpdateHeroAsync(Guid id, UpdateHeroDto dto)
        {
            try
            {
                var hero = await _heroRepository.GetByIdAsync(id);
                if (hero == null)
                    return _serviceHelper.HandleNotFound<bool>("Hero không tồn tại", $"Update Hero | Id: {id}");

                _mapper.Map(dto, hero);
                _heroRepository.Update(hero);
                await _heroRepository.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<bool>(ex, $"Update Hero | Id: {id}");
            }
        }
    }
}
