using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Application.Services;
using WorkFlowCore.Domain.Common;
using WorkFlowCore.Domain.Data;
using WorkFlowCore.Domain.Entities;

namespace WorkFlowCore.Infrastructure.Services;

/// <summary>
/// 通知公告服务实现
/// </summary>
public class NoticeService : ApplicationService, INoticeService
{
    private readonly IRepository<Notice, long> _noticeRepository;

    public NoticeService(IRepository<Notice, long> noticeRepository)
    {
        _noticeRepository = noticeRepository;
    }

    public async Task<PagedResponse<NoticeDto>> GetPagedListAsync(NoticeQueryDto query)
    {
        var queryable = await _noticeRepository.GetQueryableAsync();

        // 条件过滤
        if (!string.IsNullOrEmpty(query.NoticeTitle))
        {
            queryable = queryable.Where(n => n.NoticeTitle.Contains(query.NoticeTitle));
        }
        if (query.NoticeType.HasValue)
        {
            queryable = queryable.Where(n => n.NoticeType == query.NoticeType.Value);
        }
        if (!string.IsNullOrEmpty(query.Publisher))
        {
            queryable = queryable.Where(n => n.Publisher != null && n.Publisher.Contains(query.Publisher));
        }
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(n => n.Status == query.Status.Value);
        }

        // 排序（最新的在前）
        queryable = queryable.OrderByDescending(n => n.CreationTime);

        // 计算总数
        var totalCount = queryable.Count();

        // 分页
        var notices = queryable
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();

        var dtos = notices.Select(MapToDto).ToList();

        return new PagedResponse<NoticeDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<NoticeDto?> GetByIdAsync(long id)
    {
        var notice = await _noticeRepository.FindAsync(id);
        return notice == null ? null : MapToDto(notice);
    }

    public async Task<NoticeDto> CreateAsync(NoticeDto dto)
    {
        var notice = new Notice(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, dto.NoticeTitle)
        {
            NoticeType = dto.NoticeType,
            NoticeContent = dto.NoticeContent,
            Status = dto.Status,
            Publisher = dto.Publisher,
            BeginTime = dto.BeginTime,
            EndTime = dto.EndTime,
            Popup = dto.Popup
        };

        await _noticeRepository.InsertAsync(notice);
        return MapToDto(notice);
    }

    public async Task UpdateAsync(NoticeDto dto)
    {
        var notice = await _noticeRepository.GetAsync(dto.Id);

        notice.NoticeTitle = dto.NoticeTitle;
        notice.NoticeType = dto.NoticeType;
        notice.NoticeContent = dto.NoticeContent;
        notice.Status = dto.Status;
        notice.Publisher = dto.Publisher;
        notice.BeginTime = dto.BeginTime;
        notice.EndTime = dto.EndTime;
        notice.Popup = dto.Popup;

        await _noticeRepository.UpdateAsync(notice);
    }

    public async Task DeleteAsync(long[] ids)
    {
        await _noticeRepository.DeleteManyAsync(ids);
    }

    public async Task<List<NoticeDto>> GetPopupNoticesAsync()
    {
        var now = DateTime.Now;
        var notices = await _noticeRepository.GetListAsync(n =>
            n.Status == 0 &&
            n.Popup == 1 &&
            (n.BeginTime == null || n.BeginTime <= now) &&
            (n.EndTime == null || n.EndTime >= now)
        );

        return notices.OrderByDescending(n => n.CreationTime).Select(MapToDto).ToList();
    }

    /// <summary>
    /// 映射到DTO
    /// </summary>
    private NoticeDto MapToDto(Notice notice)
    {
        return new NoticeDto
        {
            Id = notice.Id,
            NoticeId = notice.Id,
            NoticeTitle = notice.NoticeTitle,
            NoticeType = notice.NoticeType,
            NoticeContent = notice.NoticeContent,
            Status = notice.Status,
            Publisher = notice.Publisher,
            BeginTime = notice.BeginTime,
            EndTime = notice.EndTime,
            Popup = notice.Popup,
            CreationTime = notice.CreationTime
        };
    }
}

