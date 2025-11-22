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

        // 流程定义映射
        CreateMap<ProcessDefinition, ProcessDefinitionDto>().ReverseMap();
        CreateMap<ProcessDefinition, ProcessDefinitionListDto>();
        CreateMap<ProcessDefinition, ProcessDefinitionVersionDto>();
        CreateMap<CreateProcessDefinitionRequest, ProcessDefinition>();

        // 文件存储提供者映射
        CreateMap<FileStorageProvider, FileStorageProviderDto>();
        CreateMap<CreateFileStorageProviderRequest, FileStorageProvider>();

        // 文件附件映射
        CreateMap<FileAttachment, FileAttachmentDto>();
    }
}

