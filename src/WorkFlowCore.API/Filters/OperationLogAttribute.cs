namespace WorkFlowCore.API.Filters;

/// <summary>
/// 操作日志特性
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class OperationLogAttribute : Attribute
{
    /// <summary>
    /// 操作标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 业务类型
    /// </summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>
    /// 是否保存请求参数
    /// </summary>
    public bool SaveRequest { get; set; } = true;

    /// <summary>
    /// 是否保存响应结果
    /// </summary>
    public bool SaveResponse { get; set; } = true;

    public OperationLogAttribute(string title, string businessType = "OTHER")
    {
        Title = title;
        BusinessType = businessType;
    }
}

