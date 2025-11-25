using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 登录日志服务接口
/// </summary>
public interface ILoginLogService : IApplicationService
{
    /// <summary>
    /// 获取登录日志分页列表
    /// </summary>
    Task<PagedResponse<LoginLogDto>> GetPagedListAsync(LoginLogQueryDto query);

    /// <summary>
    /// 添加登录日志
    /// </summary>
    Task AddLoginLogAsync(string userName, Guid? userId, string status, string ipaddr, string msg, string browser = "", string os = "");

    /// <summary>
    /// 删除登录日志
    /// </summary>
    Task DeleteAsync(long[] ids);

    /// <summary>
    /// 清空登录日志
    /// </summary>
    Task TruncateAsync();

    /// <summary>
    /// 获取登录日志统计（最近7天）
    /// </summary>
    Task<List<LoginLogStatDto>> GetStatisticsAsync();
}

/// <summary>
/// 登录日志统计DTO
/// </summary>
public class LoginLogStatDto
{
    public DateTime Date { get; set; }
    public int Num { get; set; }
}

