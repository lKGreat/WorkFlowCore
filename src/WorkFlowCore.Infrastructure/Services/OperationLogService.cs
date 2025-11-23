using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

public class OperationLogService : ApplicationService, IOperationLogService
{
    private readonly IRepository<OperationLog, long> _logRepository;

    public OperationLogService(IRepository<OperationLog, long> logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task<PagedResponse<OperationLogDto>> GetPagedAsync(OperationLogPagedRequest request)
    {
        var query = await _logRepository.GetQueryableAsync();

        // 条件过滤
        if (!string.IsNullOrEmpty(request.Title))
        {
            query = query.Where(l => l.Title.Contains(request.Title));
        }

        if (!string.IsNullOrEmpty(request.BusinessType))
        {
            query = query.Where(l => l.BusinessType == request.BusinessType);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(l => l.Status == request.Status);
        }

        if (!string.IsNullOrEmpty(request.OperatorName))
        {
            query = query.Where(l => l.OperatorName != null && l.OperatorName.Contains(request.OperatorName));
        }

        if (request.BeginTime.HasValue)
        {
            query = query.Where(l => l.CreationTime >= request.BeginTime);
        }

        if (request.EndTime.HasValue)
        {
            query = query.Where(l => l.CreationTime <= request.EndTime);
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.CreationTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new OperationLogDto
            {
                OperLogId = l.Id,
                Title = l.Title,
                BusinessType = l.BusinessType,
                RequestMethod = l.RequestMethod,
                RequestUrl = l.RequestUrl,
                OperatorName = l.OperatorName,
                OperatorIp = l.OperatorIp,
                ExecutionTime = l.ExecutionTime,
                Status = l.Status,
                ErrorMsg = l.ErrorMsg,
                CreationTime = l.CreationTime
            })
            .ToListAsync();

        return new PagedResponse<OperationLogDto>
        {
            Items = items,
            TotalCount = total,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<OperationLogDto?> GetByIdAsync(long id)
    {
        var log = await _logRepository.FindAsync(id);
        if (log == null) return null;

        return new OperationLogDto
        {
            OperLogId = log.Id,
            Title = log.Title,
            BusinessType = log.BusinessType,
            RequestMethod = log.RequestMethod,
            RequestUrl = log.RequestUrl,
            OperatorName = log.OperatorName,
            OperatorIp = log.OperatorIp,
            ExecutionTime = log.ExecutionTime,
            Status = log.Status,
            ErrorMsg = log.ErrorMsg,
            CreationTime = log.CreationTime
        };
    }

    public async Task DeleteAsync(List<long> ids)
    {
        await _logRepository.DeleteManyAsync(ids);
    }

    public async Task CleanAsync()
    {
        await _logRepository.DeleteAsync(_ => true);
    }
}

