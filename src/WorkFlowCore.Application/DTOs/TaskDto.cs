using Volo.Abp.Application.Dtos;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 定时任务DTO
/// </summary>
public class TaskDto : FullAuditedEntityDto<long>
{
    public string TaskName { get; set; } = string.Empty;
    public string TaskGroup { get; set; } = "DEFAULT";
    public string InvokeTarget { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Concurrent { get; set; }
    public int MisfirePolicy { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 任务查询DTO
/// </summary>
public class TaskQueryDto : PagedRequest
{
    public string? TaskName { get; set; }
    public string? TaskGroup { get; set; }
    public int? Status { get; set; }
}

/// <summary>
/// 任务日志DTO
/// </summary>
public class TaskLogDto
{
    public long Id { get; set; }
    public long TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskGroup { get; set; } = string.Empty;
    public string InvokeTarget { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? LogInfo { get; set; }
    public string? Exception { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Duration { get; set; }
    public DateTime CreationTime { get; set; }
}

/// <summary>
/// 任务日志查询DTO
/// </summary>
public class TaskLogQueryDto : PagedRequest
{
    public long? TaskId { get; set; }
    public string? TaskName { get; set; }
    public int? Status { get; set; }
    public DateTime? BeginTime { get; set; }
    public DateTime? EndTime { get; set; }
}

