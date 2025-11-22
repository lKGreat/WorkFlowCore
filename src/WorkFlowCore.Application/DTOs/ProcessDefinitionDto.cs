namespace WorkFlowCore.Application.DTOs;

/// <summary>
/// 流程定义 DTO
/// </summary>
public class ProcessDefinitionDto
{
    /// <summary>
    /// 流程ID
    /// </summary>
    public Guid Id { get; set; }

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
    public Guid TenantId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 创建流程定义请求
/// </summary>
public class CreateProcessDefinitionRequest
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
public class ProcessDefinitionListDto
{
    /// <summary>
    /// 流程ID
    /// </summary>
    public Guid Id { get; set; }

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
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 流程定义版本 DTO
/// </summary>
public class ProcessDefinitionVersionDto
{
    /// <summary>
    /// 流程ID
    /// </summary>
    public Guid Id { get; set; }

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
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

