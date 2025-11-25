using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 服务器监控服务接口
/// </summary>
public interface IServerMonitorService : IApplicationService
{
    /// <summary>
    /// 获取服务器信息
    /// </summary>
    Task<ServerInfoDto> GetServerInfoAsync();
}

