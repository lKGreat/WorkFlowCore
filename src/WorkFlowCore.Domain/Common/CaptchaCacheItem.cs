namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 图形验证码缓存项
/// </summary>
public class CaptchaCacheItem
{
    /// <summary>
    /// 验证码内容
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}

