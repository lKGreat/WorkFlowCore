using Volo.Abp.Domain.Entities.Auditing;

namespace WorkFlowCore.Domain.Entities;

/// <summary>
/// 字典类型实体
/// </summary>
public class DictType : FullAuditedAggregateRoot<long>
{
    /// <summary>
    /// 字典名称
    /// </summary>
    public string DictName { get; set; } = string.Empty;

    /// <summary>
    /// 字典类型(唯一键)
    /// </summary>
    public string DictTypeCode { get; set; } = string.Empty;

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 字典数据集合
    /// </summary>
    public virtual ICollection<DictData>? DictDatas { get; set; }

    protected DictType()
    {
    }

    public DictType(long id, string dictName, string dictTypeCode) : base(id)
    {
        DictName = dictName;
        DictTypeCode = dictTypeCode;
    }
}

