using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

public class DictService : ApplicationService, IDictService
{
    private readonly IRepository<DictType, long> _dictTypeRepository;
    private readonly IRepository<DictData, long> _dictDataRepository;

    public DictService(
        IRepository<DictType, long> dictTypeRepository,
        IRepository<DictData, long> dictDataRepository)
    {
        _dictTypeRepository = dictTypeRepository;
        _dictDataRepository = dictDataRepository;
    }

    public async Task<PagedResponse<DictTypeDto>> GetTypesPagedAsync(PagedRequest request)
    {
        var query = await _dictTypeRepository.GetQueryableAsync();
        var total = await query.CountAsync();
        
        var items = await query
            .OrderBy(d => d.DictTypeCode)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DictTypeDto
            {
                DictId = d.Id,
                DictName = d.DictName,
                DictType = d.DictTypeCode,
                Status = d.Status,
                Remark = d.Remark,
                CreationTime = d.CreationTime
            })
            .ToListAsync();

        return new PagedResponse<DictTypeDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<DictTypeDto?> GetTypeByIdAsync(long id)
    {
        var dictType = await _dictTypeRepository.FindAsync(id);
        if (dictType == null) return null;

        return new DictTypeDto
        {
            DictId = dictType.Id,
            DictName = dictType.DictName,
            DictType = dictType.DictTypeCode,
            Status = dictType.Status,
            Remark = dictType.Remark,
            CreationTime = dictType.CreationTime
        };
    }

    public async Task<DictTypeDto> CreateTypeAsync(DictTypeDto dto)
    {
        var dictType = new DictType(
            SnowflakeIdGenerator.NextId(),
            dto.DictName,
            dto.DictType)
        {
            Status = dto.Status,
            Remark = dto.Remark
        };

        await _dictTypeRepository.InsertAsync(dictType);

        dto.DictId = dictType.Id;
        dto.CreationTime = dictType.CreationTime;
        return dto;
    }

    public async Task UpdateTypeAsync(DictTypeDto dto)
    {
        var dictType = await _dictTypeRepository.GetAsync(dto.DictId);
        dictType.DictName = dto.DictName;
        dictType.DictTypeCode = dto.DictType;
        dictType.Status = dto.Status;
        dictType.Remark = dto.Remark;

        await _dictTypeRepository.UpdateAsync(dictType);
    }

    public async Task DeleteTypeAsync(List<long> ids)
    {
        // 批量删除类型及关联数据（性能优化）
        await _dictDataRepository.DeleteAsync(d => ids.Contains(d.DictTypeId));
        await _dictTypeRepository.DeleteManyAsync(ids);
    }

    public async Task<PagedResponse<DictDataDto>> GetDataPagedAsync(PagedRequest request, long? dictTypeId = null)
    {
        var query = await _dictDataRepository.GetQueryableAsync();
        
        if (dictTypeId.HasValue)
        {
            query = query.Where(d => d.DictTypeId == dictTypeId.Value);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.DictSort)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DictDataDto
            {
                DictCode = d.Id,
                DictTypeId = d.DictTypeId,
                DictLabel = d.DictLabel,
                DictValue = d.DictValue,
                DictSort = d.DictSort,
                Status = d.Status,
                CssClass = d.CssClass,
                ListClass = d.ListClass,
                IsDefault = d.IsDefault,
                CreationTime = d.CreationTime
            })
            .ToListAsync();

        return new PagedResponse<DictDataDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<List<DictDataDto>> GetDataByTypeCodeAsync(string dictTypeCode)
    {
        // 批量查询（性能优化）
        var dictType = await _dictTypeRepository.FirstOrDefaultAsync(d => d.DictTypeCode == dictTypeCode);
        if (dictType == null) return new List<DictDataDto>();

        var dataList = await _dictDataRepository.GetListAsync(d => 
            d.DictTypeId == dictType.Id && d.Status == "0");

        return dataList.OrderBy(d => d.DictSort).Select(d => new DictDataDto
        {
            DictCode = d.Id,
            DictTypeId = d.DictTypeId,
            DictLabel = d.DictLabel,
            DictValue = d.DictValue,
            DictSort = d.DictSort,
            Status = d.Status,
            CssClass = d.CssClass,
            ListClass = d.ListClass,
            IsDefault = d.IsDefault,
            CreationTime = d.CreationTime
        }).ToList();
    }

    public async Task<DictDataDto?> GetDataByIdAsync(long id)
    {
        var dictData = await _dictDataRepository.FindAsync(id);
        if (dictData == null) return null;

        return new DictDataDto
        {
            DictCode = dictData.Id,
            DictTypeId = dictData.DictTypeId,
            DictLabel = dictData.DictLabel,
            DictValue = dictData.DictValue,
            DictSort = dictData.DictSort,
            Status = dictData.Status,
            CssClass = dictData.CssClass,
            ListClass = dictData.ListClass,
            IsDefault = dictData.IsDefault,
            CreationTime = dictData.CreationTime
        };
    }

    public async Task<DictDataDto> CreateDataAsync(DictDataDto dto)
    {
        var dictData = new DictData(
            SnowflakeIdGenerator.NextId(),
            dto.DictTypeId,
            dto.DictLabel,
            dto.DictValue)
        {
            DictSort = dto.DictSort,
            Status = dto.Status,
            CssClass = dto.CssClass,
            ListClass = dto.ListClass,
            IsDefault = dto.IsDefault
        };

        await _dictDataRepository.InsertAsync(dictData);

        dto.DictCode = dictData.Id;
        dto.CreationTime = dictData.CreationTime;
        return dto;
    }

    public async Task UpdateDataAsync(DictDataDto dto)
    {
        var dictData = await _dictDataRepository.GetAsync(dto.DictCode);
        dictData.DictLabel = dto.DictLabel;
        dictData.DictValue = dto.DictValue;
        dictData.DictSort = dto.DictSort;
        dictData.Status = dto.Status;
        dictData.CssClass = dto.CssClass;
        dictData.ListClass = dto.ListClass;
        dictData.IsDefault = dto.IsDefault;

        await _dictDataRepository.UpdateAsync(dictData);
    }

    public async Task DeleteDataAsync(List<long> ids)
    {
        await _dictDataRepository.DeleteManyAsync(ids);
    }
}

