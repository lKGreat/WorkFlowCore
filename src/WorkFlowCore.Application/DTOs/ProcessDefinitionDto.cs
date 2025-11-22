using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 流程定义 DTO
/// </summary>
public class ProcessDefinitionDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码（唯一标识）
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 流程定义内容（BPMN XML 或 JSON）
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 内容格式（BPMN、JSON）
    /// </summary>
    public string ContentFormat { get; set; } = "JSON";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 租户ID
    /// </summary>
    public Guid? TenantId { get; set; }
}

/// <summary>
/// 创建流程定义请求
/// </summary>
public class CreateProcessDefinitionRequest
{
    /// <summary>
    /// 流程名称
    /// </summary>
    [Required(ErrorMessage = "流程名称不能为空")]
    [StringLength(100, ErrorMessage = "流程名称长度不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码（唯一标识）
    /// </summary>
    [Required(ErrorMessage = "流程编码不能为空")]
    [StringLength(50, ErrorMessage = "流程编码长度不能超过50个字符")]
    [RegularExpression(@"^[a-z][a-z0-9-]*$", ErrorMessage = "流程编码必须以小写字母开头，只能包含小写字母、数字和连字符")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [StringLength(500, ErrorMessage = "描述长度不能超过500个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 流程定义内容（BPMN XML 或 JSON）
    /// </summary>
    [Required(ErrorMessage = "流程定义内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 内容格式（BPMN、JSON）
    /// </summary>
    [Required(ErrorMessage = "内容格式不能为空")]
    [RegularExpression(@"^(BPMN|JSON)$", ErrorMessage = "内容格式只能是BPMN或JSON")]
    public string ContentFormat { get; set; } = "JSON";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 更新流程定义请求
/// </summary>
public class UpdateProcessDefinitionRequest
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 流程定义内容（BPMN XML 或 JSON）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 内容格式（BPMN、JSON）
    /// </summary>
    public string? ContentFormat { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool? IsEnabled { get; set; }
}

/// <summary>
/// 流程定义列表 DTO
/// </summary>
public class ProcessDefinitionListDto : EntityDto<Guid>
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 内容格式
    /// </summary>
    public string ContentFormat { get; set; } = "JSON";

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}

/// <summary>
/// 流程定义版本 DTO
/// </summary>
public class ProcessDefinitionVersionDto : EntityDto<Guid>
{
    /// <summary>
    /// 流程名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 流程编码
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}

