using AutoMapper;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Application.Mappings;

/// <summary>
/// AutoMapper 映射配置
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Tenant, TenantDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Department, DepartmentDto>().ReverseMap();
    }
}

