namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 二维码登录缓存项
/// </summary>
public class QrCodeCacheItem
{
    /// <summary>
    /// 状态标识
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 二维码状态
    /// </summary>
    public QrCodeStatus Status { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// 扫描用户ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}

/// <summary>
/// 二维码状态
/// </summary>
public enum QrCodeStatus
{
    /// <summary>
    /// 等待扫描
    /// </summary>
    WaitScan,

    /// <summary>
    /// 已扫描
    /// </summary>
    Scanned,

    /// <summary>
    /// 已确认
    /// </summary>
    Confirmed,

    /// <summary>
    /// 已过期
    /// </summary>
    Expired,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}

