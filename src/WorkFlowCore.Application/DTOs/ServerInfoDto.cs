namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 服务器信息DTO
/// </summary>
public class ServerInfoDto
{
    /// <summary>
    /// CPU信息
    /// </summary>
    public CpuInfo Cpu { get; set; } = new();

    /// <summary>
    /// 内存信息
    /// </summary>
    public MemoryInfo Memory { get; set; } = new();

    /// <summary>
    /// 磁盘信息
    /// </summary>
    public List<DiskInfo> Disks { get; set; } = new();

    /// <summary>
    /// 系统信息
    /// </summary>
    public SystemInfo System { get; set; } = new();
}

/// <summary>
/// CPU信息
/// </summary>
public class CpuInfo
{
    /// <summary>
    /// CPU名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 核心数
    /// </summary>
    public int Cores { get; set; }

    /// <summary>
    /// 使用率（%）
    /// </summary>
    public double UsagePercent { get; set; }
}

/// <summary>
/// 内存信息
/// </summary>
public class MemoryInfo
{
    /// <summary>
    /// 总内存（MB）
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// 已使用（MB）
    /// </summary>
    public long Used { get; set; }

    /// <summary>
    /// 可用内存（MB）
    /// </summary>
    public long Free { get; set; }

    /// <summary>
    /// 使用率（%）
    /// </summary>
    public double UsagePercent { get; set; }
}

/// <summary>
/// 磁盘信息
/// </summary>
public class DiskInfo
{
    /// <summary>
    /// 磁盘名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 挂载点
    /// </summary>
    public string MountPoint { get; set; } = string.Empty;

    /// <summary>
    /// 总空间（GB）
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// 已使用（GB）
    /// </summary>
    public long Used { get; set; }

    /// <summary>
    /// 可用空间（GB）
    /// </summary>
    public long Free { get; set; }

    /// <summary>
    /// 使用率（%）
    /// </summary>
    public double UsagePercent { get; set; }
}

/// <summary>
/// 系统信息
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// 操作系统
    /// </summary>
    public string Os { get; set; } = string.Empty;

    /// <summary>
    /// 系统架构
    /// </summary>
    public string Architecture { get; set; } = string.Empty;

    /// <summary>
    /// 主机名
    /// </summary>
    public string HostName { get; set; } = string.Empty;

    /// <summary>
    /// .NET版本
    /// </summary>
    public string FrameworkVersion { get; set; } = string.Empty;

    /// <summary>
    /// 启动时间
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 运行时长（秒）
    /// </summary>
    public long RunTime { get; set; }
}

