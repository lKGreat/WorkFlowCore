namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 实体基类
/// </summary>
public abstract class Entity<TKey>
{
    /// <summary>
    /// 主键ID
    /// </summary>
    public TKey Id { get; set; } = default!;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

