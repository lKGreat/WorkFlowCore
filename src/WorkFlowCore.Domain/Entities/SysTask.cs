using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 定时任务实体
/// </summary>
public class SysTask : FullAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务组名
    /// </summary>
    public string TaskGroup { get; set; } = "DEFAULT";

    /// <summary>
    /// 调用目标（完整类名）
    /// </summary>
    public string InvokeTarget { get; set; } = string.Empty;

    /// <summary>
    /// Cron执行表达式
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// 任务状态（0=正常,1=暂停）
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 是否并发执行（0=禁止,1=允许）
    /// </summary>
    public int Concurrent { get; set; } = 1;

    /// <summary>
    /// 错误策略（1=立即执行,2=执行一次,3=放弃执行）
    /// </summary>
    public int MisfirePolicy { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected SysTask() { }

    public SysTask(long id, Guid? tenantId, string taskName) : base(id)
    {
        TenantId = tenantId;
        TaskName = taskName;
    }
}

/// <summary>
/// 任务执行日志实体
/// </summary>
public class SysTaskLog : CreationAuditedAggregateRoot<long>, IMultiTenant
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public long TaskId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// 任务组名
    /// </summary>
    public string TaskGroup { get; set; } = "DEFAULT";

    /// <summary>
    /// 调用目标
    /// </summary>
    public string InvokeTarget { get; set; } = string.Empty;

    /// <summary>
    /// 执行状态（0=成功,1=失败）
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 日志信息
    /// </summary>
    public string? LogInfo { get; set; }

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// 执行开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 执行结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; protected set; }

    protected SysTaskLog() { }

    public SysTaskLog(long id, Guid? tenantId, long taskId, string taskName) : base(id)
    {
        TenantId = tenantId;
        TaskId = taskId;
        TaskName = taskName;
    }
}

