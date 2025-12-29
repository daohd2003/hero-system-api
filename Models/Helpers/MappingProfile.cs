using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.DTOs;
using BusinessObject.Models;
using static BusinessObject.DTOs.FactionDtos;
using static BusinessObject.DTOs.MissionDtos;

namespace BusinessObject.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateHeroDto, Hero>();
            CreateMap<UpdateHeroDto, Hero>();
            CreateMap<Hero, HeroDto>()
                .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Level >= 50 ? "S-Class" : "A-Class"))
                .ForMember(dest => dest.FactionName, opt => opt.MapFrom(src => src.Faction != null ? src.Faction.Name ?? "N/A" : "N/A"));

            // Faction mappings
            CreateMap<CreateFactionDto, Faction>();
            CreateMap<Faction, FactionDto>()
                .ForMember(dest => dest.MemberCount, opt => opt.MapFrom(src => src.Heroes != null ? src.Heroes.Count : 0));
            CreateMap<CreateMissionDto, Mission>();
            CreateMap<Mission, MissionDto>();
        }
    }
}
