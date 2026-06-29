using AutoMapper;
using MetricsAPI.DTOs;
using MetricsAPI.Models;

namespace MetricsAPI.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Repository, RepositoryResponseDto>();
        CreateMap<CreateRepositoryDto, Repository>();
    }
}