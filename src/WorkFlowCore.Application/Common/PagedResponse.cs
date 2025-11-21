namespace WorkFlowCore.Application.Common;

/// <summary>
/// 分页响应结构
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// 创建分页响应
    /// </summary>
    public static PagedResponse<T> Create(List<T> items, int totalCount, int pageIndex, int pageSize)
    {
        return new PagedResponse<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 创建空分页响应
    /// </summary>
    public static PagedResponse<T> Empty(int pageIndex = 1, int pageSize = 10)
    {
        return new PagedResponse<T>
        {
            Items = new List<T>(),
            TotalCount = 0,
            PageIndex = pageIndex,
            PageSize = pageSize
        };
    }
}

