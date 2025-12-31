using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using Services.Common;

namespace Services
{
    public class MissionService : IMissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceHelper _serviceHelper;
        private readonly IMapper _mapper;

        public MissionService(IUnitOfWork unitOfWork, IServiceHelper serviceHelper, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _serviceHelper = serviceHelper;
            _mapper = mapper;
        }

        public async Task<ServiceResult<MissionDtos.MissionDto>> CreateMissionAsync(MissionDtos.CreateMissionDto dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var mission = _mapper.Map<Mission>(dto);
                await _unitOfWork.Missions.AddAsync(mission);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<MissionDtos.MissionDto>.Created(_mapper.Map<MissionDtos.MissionDto>(mission));
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<MissionDtos.MissionDto>(ex);
            }
        }

        public async Task<ServiceResult<List<MissionDtos.MissionDto>>> GetAllMissionsAsync()
        {
            try
            {
                var missions = await _unitOfWork.Missions.GetAllAsync();
                var dtos = _mapper.Map<List<MissionDtos.MissionDto>>(missions);
                return ServiceResult<List<MissionDtos.MissionDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<List<MissionDtos.MissionDto>>(ex);
            }
        }

        public async Task<ServiceResult<bool>> AssignMissionToHeroAsync(MissionDtos.AssignMissionDto dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Validate: Hero có tồn tại không?
                var hero = await _unitOfWork.Heroes.GetByIdAsync(dto.HeroId);
                if (hero == null) return _serviceHelper.HandleNotFound<bool>("Hero không tồn tại");

                // 2. Validate: Mission có tồn tại không?
                var mission = await _unitOfWork.Missions.GetByIdAsync(dto.MissionId);
                if (mission == null) return _serviceHelper.HandleNotFound<bool>("Mission không tồn tại");

                // 3. Logic nghiệp vụ: Một Hero không thể làm 1 nhiệm vụ 2 lần (Tùy game)
                var existingRecord = await _unitOfWork.Missions.GetHeroMissionAsync(dto.HeroId, dto.MissionId);
                if (existingRecord != null)
                {
                    return _serviceHelper.HandleBadRequest<bool>("Hero đã hoàn thành nhiệm vụ này rồi!");
                }

                var heroMision = new HeroMission
                {
                    HeroId = dto.HeroId,
                    MissionId = dto.MissionId,
                    CompletedDate = DateTime.UtcNow,
                    ResultRank = dto.ResultRank
                };

                await _unitOfWork.Missions.AddHeroMissionAsync(heroMision);
                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<bool>(ex);
            }
        }
    }
}
