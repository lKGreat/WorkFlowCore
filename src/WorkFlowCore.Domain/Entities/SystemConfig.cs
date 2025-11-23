using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 系统配置实体
/// </summary>
public class SystemConfig : FullAuditedAggregateRoot<long>
{
    /// <summary>
    /// 配置键
    /// </summary>
    public string ConfigKey { get; set; } = string.Empty;

    /// <summary>
    /// 配置值
    /// </summary>
    public string ConfigValue { get; set; } = string.Empty;

    /// <summary>
    /// 配置名称
    /// </summary>
    public string ConfigName { get; set; } = string.Empty;

    /// <summary>
    /// 配置类型 (Y=系统内置, N=自定义)
    /// </summary>
    public string ConfigType { get; set; } = "N";

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    protected SystemConfig()
    {
    }

    public SystemConfig(long id, string configKey, string configValue, string configName) : base(id)
    {
        ConfigKey = configKey;
        ConfigValue = configValue;
        ConfigName = configName;
    }
}

