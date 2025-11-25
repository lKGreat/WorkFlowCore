using Volo.Abp.Application.Dtos;
using WorkFlowCore.Domain.Common;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 岗位DTO
/// </summary>
public class PostDto : FullAuditedEntityDto<long>
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string PostCode { get; set; } = string.Empty;

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string PostName { get; set; } = string.Empty;

    /// <summary>
    /// 显示顺序
    /// </summary>
    public int PostSort { get; set; }

    /// <summary>
    /// 状态 (0=正常, 1=停用)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 用户数量（仅查询时使用）
    /// </summary>
    public int? UserNum { get; set; }
}

/// <summary>
/// 岗位查询DTO
/// </summary>
public class PostQueryDto : PagedRequest
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    public string? PostCode { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string? PostName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string? Status { get; set; }
}

