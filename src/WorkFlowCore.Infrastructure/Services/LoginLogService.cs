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
/// 登录日志服务实现
/// </summary>
public class LoginLogService : ApplicationService, ILoginLogService
{
    private readonly IRepository<LoginLog, long> _loginLogRepository;

    public LoginLogService(IRepository<LoginLog, long> loginLogRepository)
    {
        _loginLogRepository = loginLogRepository;
    }

    public async Task<PagedResponse<LoginLogDto>> GetPagedListAsync(LoginLogQueryDto query)
    {
        var queryable = await _loginLogRepository.GetQueryableAsync();

        // 条件过滤
        if (!string.IsNullOrEmpty(query.UserName))
        {
            queryable = queryable.Where(l => l.UserName.Contains(query.UserName));
        }
        if (query.UserId.HasValue)
        {
            queryable = queryable.Where(l => l.UserId == query.UserId.Value);
        }
        if (!string.IsNullOrEmpty(query.Status))
        {
            queryable = queryable.Where(l => l.Status == query.Status);
        }
        if (!string.IsNullOrEmpty(query.Ipaddr))
        {
            queryable = queryable.Where(l => l.Ipaddr.Contains(query.Ipaddr));
        }
        if (query.BeginTime.HasValue)
        {
            queryable = queryable.Where(l => l.LoginTime >= query.BeginTime.Value);
        }
        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(l => l.LoginTime <= query.EndTime.Value);
        }

        // 排序（最新的在前）
        queryable = queryable.OrderByDescending(l => l.LoginTime);

        // 计算总数
        var totalCount = queryable.Count();

        // 分页
        var logs = queryable
            .Skip(query.Skip)
            .Take(query.Take)
            .ToList();

        var dtos = logs.Select(MapToDto).ToList();

        return new PagedResponse<LoginLogDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task AddLoginLogAsync(string userName, Guid? userId, string status, string ipaddr, string msg, string browser = "", string os = "")
    {
        var log = new LoginLog(SnowflakeIdGenerator.NextId(), CurrentTenant.Id, userName)
        {
            UserId = userId,
            Status = status,
            Ipaddr = ipaddr,
            Msg = msg,
            Browser = browser,
            Os = os,
            LoginTime = DateTime.Now
        };

        await _loginLogRepository.InsertAsync(log);
    }

    public async Task DeleteAsync(long[] ids)
    {
        await _loginLogRepository.DeleteManyAsync(ids);
    }

    public async Task TruncateAsync()
    {
        var allLogs = await _loginLogRepository.GetListAsync();
        await _loginLogRepository.DeleteManyAsync(allLogs);
    }

    public async Task<List<LoginLogStatDto>> GetStatisticsAsync()
    {
        var queryable = await _loginLogRepository.GetQueryableAsync();
        
        // 获取最近7天的数据
        var startDate = DateTime.Now.Date.AddDays(-6);
        var logs = queryable.Where(l => l.LoginTime >= startDate).ToList();

        // 按日期分组统计
        var stats = logs
            .GroupBy(l => l.LoginTime.Date)
            .Select(g => new LoginLogStatDto
            {
                Date = g.Key,
                Num = g.Count()
            })
            .OrderBy(s => s.Date)
            .ToList();

        // 补全7天数据（没有登录记录的日期也要显示）
        var result = new List<LoginLogStatDto>();
        for (int i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var stat = stats.FirstOrDefault(s => s.Date == date);
            result.Add(stat ?? new LoginLogStatDto { Date = date, Num = 0 });
        }

        return result;
    }

    /// <summary>
    /// 映射到DTO
    /// </summary>
    private LoginLogDto MapToDto(LoginLog log)
    {
        return new LoginLogDto
        {
            InfoId = log.Id,
            UserName = log.UserName,
            UserId = log.UserId,
            Status = log.Status,
            Ipaddr = log.Ipaddr,
            LoginLocation = log.LoginLocation,
            Browser = log.Browser,
            Os = log.Os,
            Msg = log.Msg,
            LoginTime = log.LoginTime,
            ClientId = log.ClientId
        };
    }
}

