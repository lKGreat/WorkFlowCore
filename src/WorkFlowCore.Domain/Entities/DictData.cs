using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 字典数据实体
/// </summary>
public class DictData : FullAuditedAggregateRoot<long>
{
    /// <summary>
    /// 字典类型ID
    /// </summary>
    public long DictTypeId { get; set; }

    /// <summary>
    /// 字典标签
    /// </summary>
    public string DictLabel { get; set; } = string.Empty;

    /// <summary>
    /// 字典值
    /// </summary>
    public string DictValue { get; set; } = string.Empty;

    /// <summary>
    /// 字典排序
    /// </summary>
    public int DictSort { get; set; }

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// CSS类名
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// 表格样式
    /// </summary>
    public string? ListClass { get; set; }

    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// 字典类型导航属性
    /// </summary>
    public virtual DictType? DictType { get; set; }

    protected DictData()
    {
    }

    public DictData(long id, long dictTypeId, string dictLabel, string dictValue) : base(id)
    {
        DictTypeId = dictTypeId;
        DictLabel = dictLabel;
        DictValue = dictValue;
    }
}

