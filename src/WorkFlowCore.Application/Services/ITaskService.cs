using Volo.Abp.Application.Services;
using WorkFlowCore.Application.DTOs;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.Services;

/// <summary>
/// 定时任务服务接口（简化版）
/// </summary>
public interface ITaskService : IApplicationService
{
    Task<PagedResponse<TaskDto>> GetPagedListAsync(TaskQueryDto query);
    Task<TaskDto?> GetByIdAsync(long id);
    Task<TaskDto> CreateAsync(TaskDto dto);
    Task UpdateAsync(TaskDto dto);
    Task DeleteAsync(long[] ids);
    Task<PagedResponse<TaskLogDto>> GetLogPagedListAsync(TaskLogQueryDto query);
    Task ClearLogsAsync(long taskId);
}

