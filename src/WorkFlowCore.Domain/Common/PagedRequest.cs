namespace WorkFlowCore.Domain.Common;

/// <summary>
/// 分页请求基类
/// </summary>
public class PagedRequest
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = value < 1 ? 1 : value;
    }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 10 : (value > 100 ? 100 : value);
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool Descending { get; set; }

    /// <summary>
    /// 搜索关键词
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 计算跳过的记录数
    /// </summary>
    public int Skip => (PageIndex - 1) * PageSize;

    /// <summary>
    /// 获取数量
    /// </summary>
    public int Take => PageSize;
}

