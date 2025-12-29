using AutoMapper;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Repositories;
using Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FactionService : IFactionService
    {
        private readonly IFactionRepository _factionRepository;
        private readonly IMapper _mapper;
        private readonly IServiceHelper _serviceHelper;

        public FactionService(IFactionRepository factionRepository, IMapper mapper, IServiceHelper serviceHelper)
        {
            _factionRepository = factionRepository;
            _mapper = mapper;
            _serviceHelper = serviceHelper;
        }

        public async Task<ServiceResult<FactionDtos.FactionDto>> CreateFactionAsync(FactionDtos.CreateFactionDto dto)
        {
            await using var transaction = await _factionRepository.BeginTransactionAsync();
            try
            {
                var faction = _mapper.Map<Faction>(dto);
                faction.Id = Guid.NewGuid();

                await _factionRepository.AddAsync(faction);
                await _factionRepository.SaveChangesAsync();

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
                var faction = await _factionRepository.GetByIdAsync(id);
                if (faction == null)
                    return _serviceHelper.HandleNotFound<bool>("Faction not found");

                _factionRepository.DeleteAsync(faction);
                await _factionRepository.SaveChangesAsync();

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
                var factions = await _factionRepository.GetAllAsync();
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
