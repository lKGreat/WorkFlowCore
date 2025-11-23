using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 操作日志实体
/// </summary>
public class OperationLog : FullAuditedEntity<long>
{
    /// <summary>
    /// 操作标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型 (INSERT, UPDATE, DELETE, EXPORT等)
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 请求方法
    /// </summary>
    public string RequestMethod { get; set; } = string.Empty;

    /// <summary>
    /// 请求URL
    /// </summary>
    public string RequestUrl { get; set; } = string.Empty;

    /// <summary>
    /// 请求参数
    /// </summary>
    public string? RequestParams { get; set; }

    /// <summary>
    /// 响应结果
    /// </summary>
    public string? ResponseResult { get; set; }

    /// <summary>
    /// 执行时间(毫秒)
    /// </summary>
    public int ExecutionTime { get; set; }

    /// <summary>
    /// 状态 (0=成功, 1=失败)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 错误消息
    /// </summary>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 操作人员
    /// </summary>
    public string? OperatorName { get; set; }

    /// <summary>
    /// 操作IP
    /// </summary>
    public string? OperatorIp { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    public string? OperatorLocation { get; set; }

    protected OperationLog()
    {
    }

    public OperationLog(long id, string title, string businessType) : base(id)
    {
        Title = title;
        BusinessType = businessType;
    }
}

