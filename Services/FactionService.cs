using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using Services.Common;

namespace Services
{
    public class FactionService : IFactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IServiceHelper _serviceHelper;

        public FactionService(IUnitOfWork unitOfWork, IMapper mapper, IServiceHelper serviceHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _serviceHelper = serviceHelper;
        }

        public async Task<ServiceResult<FactionDtos.FactionDto>> CreateFactionAsync(FactionDtos.CreateFactionDto dto)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var faction = _mapper.Map<Faction>(dto);
                faction.Id = Guid.NewGuid();

                await _unitOfWork.Factions.AddAsync(faction);
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return ServiceResult<FactionDtos.FactionDto>.Created(_mapper.Map<FactionDtos.FactionDto>(faction));
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<FactionDtos.FactionDto>(ex);
            }
        }

        public async Task<ServiceResult<bool>> DeleteFactionAsync(Guid id)
        {
            try
            {
                var faction = await _unitOfWork.Factions.GetByIdAsync(id);
                if (faction == null)
                    return _serviceHelper.HandleNotFound<bool>("Faction not found");

                _unitOfWork.Factions.DeleteAsync(faction);
                await _unitOfWork.SaveChangesAsync();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<bool>(ex);
            }
        }

        public async Task<ServiceResult<List<FactionDtos.FactionDto>>> GetAllFactionsAsync()
        {
            try
            {
                var factions = await _unitOfWork.Factions.GetAllAsync();
                var dtos = _mapper.Map<List<FactionDtos.FactionDto>>(factions);
                return ServiceResult<List<FactionDtos.FactionDto>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                return _serviceHelper.HandleError<List<FactionDtos.FactionDto>>(ex);
            }
        }
    }
}
