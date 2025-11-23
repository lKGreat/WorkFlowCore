using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

public interface IDictService : IApplicationService
{
    // 字典类型
    Task<PagedResponse<DictTypeDto>> GetTypesPagedAsync(PagedRequest request);
    Task<DictTypeDto?> GetTypeByIdAsync(long id);
    Task<DictTypeDto> CreateTypeAsync(DictTypeDto dto);
    Task UpdateTypeAsync(DictTypeDto dto);
    Task DeleteTypeAsync(List<long> ids);
    
    // 字典数据
    Task<PagedResponse<DictDataDto>> GetDataPagedAsync(PagedRequest request, long? dictTypeId = null);
    Task<List<DictDataDto>> GetDataByTypeCodeAsync(string dictTypeCode);
    Task<DictDataDto?> GetDataByIdAsync(long id);
    Task<DictDataDto> CreateDataAsync(DictDataDto dto);
    Task UpdateDataAsync(DictDataDto dto);
    Task DeleteDataAsync(List<long> ids);
}

public class DictTypeDto
{
    public long DictId { get; set; }
    public string DictName { get; set; } = string.Empty;
    public string DictType { get; set; } = string.Empty;
    public string Status { get; set; } = "0";
    public string? Remark { get; set; }
    public DateTime CreationTime { get; set; }
}

public class DictDataDto
{
    public long DictCode { get; set; }
    public long DictTypeId { get; set; }
    public string DictLabel { get; set; } = string.Empty;
    public string DictValue { get; set; } = string.Empty;
    public int DictSort { get; set; }
    public string Status { get; set; } = "0";
    public string? CssClass { get; set; }
    public string? ListClass { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreationTime { get; set; }
}

